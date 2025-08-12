using MediatR;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Shared.Kernel.Constants;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Microsoft.Extensions.Configuration;

namespace E_Menu.Internal.Commands.SaveImagesInLocal;

internal class SaveImagesInLocalCommandHandler(IOrganizationServiceAsync service,IConfiguration configuration) : IRequestHandler<SaveImagesInLocalCommand>
{
    public async Task Handle(SaveImagesInLocalCommand request, CancellationToken cancellationToken)
    {
        var query = new QueryExpression
        {
            EntityName = EntityLogicalNames.MenuProduct,
            ColumnSet = new ColumnSet(MenuProductAttributes.Id),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(MenuProductAttributes.ProductImage, ConditionOperator.NotNull),
                    new ConditionExpression(MenuProductAttributes.ImageSyncDateUTC, ConditionOperator.Null)

                }
            }
        };

        var entities = service.RetrieveMultipleAsync(query).Result.Entities;
        foreach (var item in entities)
        {
            var bytes = DownloadImage(item.Id, item.LogicalName, MenuProductAttributes.ProductImage, service);

            var fileName = $"{item.Id}.jpg";
            var filePath = Path.Combine("wwwroot", "images", "items", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);

            // RESİM BOYUTLANDIRMA VE OPTİMİZASYON
            using var image = Image.Load(bytes);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(300, 300),
                Mode = ResizeMode.Max
            }));

            var encoder = new JpegEncoder
            {
                Quality = 75 // Kalite ayarı: 1-100 (isteğe göre düşürülüp artırılabilir)
            };

            await image.SaveAsJpegAsync(filePath, encoder);

            var appBaseUrl = configuration["BaseUrl"];

            var updateEntity = new Entity(item.LogicalName, item.Id)
            {
                [MenuProductAttributes.ImageUrl] = $"{appBaseUrl}/images/items/{fileName}",
                [MenuProductAttributes.ImageSyncDateUTC] = DateTime.UtcNow
            };

            await service.UpdateAsync(updateEntity);
        }
    }

    private byte[] DownloadImage(Guid itemId, string logicalName, string fileAttributeName, IOrganizationService service)
    {
        InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new InitializeFileBlocksDownloadRequest
        {
            Target = new EntityReference(logicalName, itemId),
            FileAttributeName = fileAttributeName
        };

        InitializeFileBlocksDownloadResponse initializeFileBlocksDownloadResponse = (InitializeFileBlocksDownloadResponse)service.Execute(initializeFileBlocksDownloadRequest);

        DownloadBlockRequest downloadBlockRequest = new DownloadBlockRequest { FileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken };
        DownloadBlockResponse downloadBlockResponse = (DownloadBlockResponse)service.Execute(downloadBlockRequest);
        return downloadBlockResponse.Data;
    }
}


