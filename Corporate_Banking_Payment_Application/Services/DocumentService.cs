using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            IDocumentRepository documentRepo,
            IMapper mapper,
            Cloudinary cloudinary,
            ILogger<DocumentService> logger)
        {
            _documentRepo = documentRepo;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _logger = logger;
        }

        // --- Retrieval Operations ---

        public async Task<IEnumerable<DocumentDto>> GetAllDocuments()
        {
            var documents = await _documentRepo.GetAllDocuments();
            return _mapper.Map<IEnumerable<DocumentDto>>(documents);
        }

        public async Task<DocumentDto?> GetDocumentById(int id)
        {
            var document = await _documentRepo.GetDocumentById(id);
            return document == null ? null : _mapper.Map<DocumentDto>(document);
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentsByCustomerId(int customerId)
        {
            var documents = await _documentRepo.GetDocumentsByCustomerId(customerId);
            return _mapper.Map<IEnumerable<DocumentDto>>(documents);
        }

        // --- Creation / Upload Operation ---

        public async Task<DocumentDto> UploadDocument(IFormFile file, CreateDocumentDto dto)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be empty.", nameof(file));
            }

            // 1. Upload to Cloudinary
            var uploadParams = new RawUploadParams()
            {
                // Use RawUploadParams for documents (PDF, DOCX, non-image files)
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                // Define a public ID based on customer/type for organization and security
                PublicId = $"customer-docs/{dto.CustomerId}/{dto.DocumentType}/{Guid.NewGuid()}",
                Folder = "corporate_banking_app", // Optional folder structure
                // Use "authenticated" type for sensitive banking documents
                Type = "authenticated"
            };

            // FIX: Changed ImageUploadResult to RawUploadResult as RawUploadParams was used.
            RawUploadResult uploadResult;
            try
            {
                // UploadAsync with RawUploadParams returns RawUploadResult
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file to Cloudinary for CustomerId: {CustomerId}", dto.CustomerId);
                throw new Exception("Document upload failed due to external service error.", ex);
            }

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Cloudinary upload failed with status: {Status} and message: {Message}",
                    uploadResult.StatusCode, uploadResult.Error?.Message);
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
            }


            // 2. Map DTO to Model and populate Cloudinary fields
            var document = _mapper.Map<Document>(dto);

            document.DocumentName = file.FileName;
            document.FileSize = file.Length;
            document.CloudinaryPublicId = uploadResult.PublicId;
            document.FileUrl = uploadResult.SecureUrl.ToString(); // Store the secure URL

            // 3. Save model to database
            var createdDocument = await _documentRepo.AddDocument(document);

            // 4. Return DTO
            return _mapper.Map<DocumentDto>(createdDocument);
        }

        // --- Update Operation ---

        public async Task<DocumentDto?> UpdateDocument(int id, UpdateDocumentDto dto)
        {
            var existing = await _documentRepo.GetDocumentById(id);
            if (existing == null) return null;

            // Only update DocumentType and IsActive (per DTO definition)
            _mapper.Map(dto, existing);

            await _documentRepo.UpdateDocument(existing);
            return _mapper.Map<DocumentDto>(existing);
        }

        // --- Deletion Operation ---

        public async Task<bool> DeleteDocument(int id)
        {
            var document = await _documentRepo.GetDocumentById(id);
            if (document == null) return false;

            // 1. Delete file from Cloudinary first
            var deletionParams = new DeletionParams(document.CloudinaryPublicId)
            {
                ResourceType = ResourceType.Raw, // Specify ResourceType.Raw for non-image documents
                Type = "authenticated"
            };

            DeletionResult deletionResult;
            try
            {
                deletionResult = await _cloudinary.DestroyAsync(deletionParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file from Cloudinary for PublicId: {PublicId}", document.CloudinaryPublicId);
                // Decide whether to proceed with DB deletion if Cloudinary fails (often you should stop here)
                throw new Exception("Cloudinary file deletion failed.", ex);
            }

            // Cloudinary deletion is successful if result is "ok" or "not_found"
            if (deletionResult.Result.ToLowerInvariant() == "ok" || deletionResult.Result.ToLowerInvariant() == "not found")
            {
                // 2. If Cloudinary deletion is confirmed, remove the database record
                await _documentRepo.DeleteDocument(document);
                return true;
            }

            _logger.LogWarning("Cloudinary deletion failed for PublicId: {PublicId}. Result: {Result}",
                document.CloudinaryPublicId, deletionResult.Result);
            return false;
        }
    }
}
