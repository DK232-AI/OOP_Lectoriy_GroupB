using System;

namespace Marketplace.Domain;

public class DigitalProduct : Product
{
    public string DownloadUrl { get; }
    public int FileSizeMb { get; }

    public DigitalProduct(string downloadUrl, int fileSizeMb)
    {
        ArgumentNullException.ThrowIfNull(downloadUrl);
        if (fileSizeMb <= 0)
            throw new ArgumentOutOfRangeException(nameof(fileSizeMb), "Размер файла должен быть больше нуля.");

        DownloadUrl = downloadUrl;
        FileSizeMb = fileSizeMb;
    }

    public string GetFullInfo() => $"{Title}: {base.GetDescription()}, размер файла: {FileSizeMb} МБ";
}
