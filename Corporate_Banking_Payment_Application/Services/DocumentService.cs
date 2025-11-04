using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using System;

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

        //public async Task<string?> GetSignedDownloadUrl(int id)
        //{
        //    // 1. Find the document in your database
        //    var document = await _documentRepo.GetDocumentById(id);

        //    if (document == null || !document.IsActive)
        //    {
        //        return null;
        //    }

        //    // 2. Determine the ResourceType.
        //    var resourceType = document.DocumentType.StartsWith("image") ? "image" : "raw";

        //    // 3. Generate the signed, authenticated URL to force download
        //    var downloadUrl = _cloudinary.Api.Url // Use _cloudinary.Api.Url
        //        .Type("authenticated")
        //        .ResourceType(resourceType)
        //        .PublicId(document.CloudinaryPublicId)
        //        .Secure(true)
        //        .SignUrl(true) // Generates the signature
        //        .Expiration(DateTime.Now.AddMinutes(15)) // URL is valid for 15 mins
        //        .Transform(new Transformation()
        //            // This forces download + sets the filename
        //            .Flags("attachment:" + document.DocumentName))
        //        .BuildUrl();

        //    // 4. Return the URL
        //    return downloadUrl;
        //}


        // --- Retrieval Operations ---

        //public async Task<IEnumerable<DocumentDto>> GetAllDocuments()
        //{
        //    var documents = await _documentRepo.GetAllDocuments();
        //    return _mapper.Map<IEnumerable<DocumentDto>>(documents);
        //}
        public async Task<PagedResult<DocumentDto>> GetAllDocuments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _documentRepo.GetAllDocuments(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);

            var itemsDto = _mapper.Map<IEnumerable<DocumentDto>>(pagedResult.Items);

            return new PagedResult<DocumentDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

    //    public async Task<string?> GetTemporaryViewUrl(int id)
    //    {
    //        var document = await _documentRepo.GetDocumentById(id);
    //        if (document == null) return null;

    //        var publicId = document.CloudinaryPublicId;
    //        var resourceType = document.DocumentType.StartsWith("image") ? "image" : "raw";
    //        var type = "authenticated";

    //        // 1. Get the current time as a Unix timestamp
    //        long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    //        // 2. Set expiration 10 minutes (600 seconds) from now
    //        long expiration = timestamp + 600;

    //        // 3. Create a dictionary for signing.
    //        // We MUST use SortedDictionary for the signature to be correct
    //        var parametersToSign = new SortedDictionary<string, object>
    //{
    //    { "public_id", publicId },
    //    { "timestamp", timestamp },
    //    { "exp", expiration }
    //    // Note: Do NOT add resource_type or type here
    //};

    //        // 4. Sign the parameters to get the signature
    //        string signature = _cloudinary.Api.SignParameters(parametersToSign);

    //        // 5. Build the base URL
    //        string baseUrl = _cloudinary.Api.Url
    //            .ResourceType(resourceType)
    //            .Type(type)
    //            .Secure(true)
    //            .BuildUrl(publicId); // This only takes the publicId

    //        // 6. Manually create the query string
    //        string queryString = $"exp={expiration}&timestamp={timestamp}&signature={signature}&api_key={_cloudinary.Api.Account.ApiKey}";

    //        // 7. Combine the base URL and the query string
    //        string finalUrl = $"{baseUrl}?{queryString}";

    //        return finalUrl;
    //    }




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
                Folder = "corporate_banking_app_documents", // Optional folder structure
                // Use "authenticated" type for sensitive banking documents
                Type = "upload"
            };

 
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
