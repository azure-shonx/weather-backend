using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

public class AzureHandler
{
    private BlobServiceClient bsc;
    private BlobContainerClient bcc;

    private BlobClient bc;

    private readonly string FILE_NAME = "emails.json";
    public AzureHandler()
    {
        bsc = new BlobServiceClient(
                new Uri("https://weatherb313.blob.core.windows.net"),
                new DefaultAzureCredential());
        bcc = bsc.GetBlobContainerClient("emails");
        bc = bcc.GetBlobClient(FILE_NAME);
    }

    public List<Email> GetEmails()
    {
        return GetEmails0().Result;

    }

    public void AddEmail(Email email)
    {
        bool updated = false;
        var emails = GetEmails();
        foreach(Email e in emails)
        {
            if (e.email == email.email)
            {
                e.zipcode = email.zipcode;
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
        Email toRemove = null;
        foreach(Email e in emails)
        {
            if (e.email == email.email)
            {
                toRemove = e;
                break;
            }
        }
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

        return JsonConvert.DeserializeObject<List<Email>>(json);
    }

    private async void SaveEmails(List<Email> emails)
    {
        string localFilePath = GetLocalFilePath();

        await File.WriteAllTextAsync(localFilePath, JsonConvert.SerializeObject(emails));

        await bc.UploadAsync(localFilePath, true);
    }

    private string GetLocalFilePath()
    {
        string fileName = Guid.NewGuid().ToString() + "_" + FILE_NAME;
        string path = Path.Combine("/app/temp/", fileName);
        return path;
    }

}