namespace net.shonx.weather.backend.handlers;

using Azure.Identity;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using System;
using System.IO;

public class AzureStorageHandler
{
    private readonly BlobServiceClient bsc;
    private readonly BlobContainerClient bcc;
    private readonly BlobClient bc;
    private const string INDEX = "emails.json";

    public AzureStorageHandler()
    {
        bsc = new BlobServiceClient(
                new Uri("https://weatherb313.blob.core.windows.net"),
                new DefaultAzureCredential());
        bcc = bsc.GetBlobContainerClient("emails");
        bc = bcc.GetBlobClient(INDEX);
    }

    public List<Email> GetEmails()
    {
        return GetEmails0().Result;
    }

    public void AddEmail(Email email)
    {
        bool updated = false;
        var emails = GetEmails();
        foreach (Email e in emails)
        {
            if (e.Value == email.Value)
            {
                e.Zipcode = email.Zipcode;
                updated = true;
            }
        }
        if (!updated)
            emails.Add(email);
        SaveEmails(emails);
    }

    public bool DeleteEmail(Email email)
    {
        bool removed = false;
        var emails = GetEmails();
        Email? toRemove = null;
        foreach (Email e in emails)
        {
            if (e.Value == email.Value)
            {
                toRemove = e;
                break;
            }
        }
        if (toRemove is null)
            return false;
        removed = emails.Remove(toRemove);
        if (removed)
            SaveEmails(emails);
        return removed;
    }


    private async Task<List<Email>> GetEmails0()
    {
        string localFilePath = GetLocalFilePath();

        await bc.DownloadToAsync(localFilePath);

        string json = await File.ReadAllTextAsync(localFilePath);

        return JsonConvert.DeserializeObject<List<Email>>(json) ?? [];
    }

    private async void SaveEmails(List<Email> emails)
    {
        string localFilePath = GetLocalFilePath();

        await File.WriteAllTextAsync(localFilePath, JsonConvert.SerializeObject(emails));

        await bc.UploadAsync(localFilePath, true);
    }

    private string GetLocalFilePath()
    {
        string fileName = Guid.NewGuid().ToString() + "_" + INDEX;
        string path = Path.Combine("/app/temp/", fileName);
        return path;
    }

}