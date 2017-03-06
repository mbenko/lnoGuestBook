using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using lnoAzureWebApp.Models;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace lnoAzureWebApp.Controllers
{
    public class GuestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Guests
        public ActionResult Index()
        {
            return View(db.GuestBookEntries.ToList());
        }

        // GET: Guests/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GuestBookEntry guestBookEntry = db.GuestBookEntries.Find(id);
            if (guestBookEntry == null)
            {
                Trace.TraceWarning($"{DateTime.UtcNow.ToShortTimeString()} > Guests/Details - WARNING: Not found");
                return HttpNotFound();
            }
            return View(guestBookEntry);
        }

        // GET: Guests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Guests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Message,GuestName,PhotoUrl,ThumbUrl,CreateDt")] GuestBookEntry guestBookEntry, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                guestBookEntry.PhotoUrl = UploadBlob(file);

                db.GuestBookEntries.Add(guestBookEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(guestBookEntry);
        }
        private string UploadBlob(HttpPostedFileBase file)
        {
            // *** BENKO: Upload image Blob
            string imageBlob = "/images/noimage.png";

            if (file != null)
            {
                // Upload file to blob storage & save URL
                Trace.WriteLine("Uploading file " + file.FileName);
                string pic = Guid.NewGuid().ToString().Substring(0, 5) + "_" + System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                        Server.MapPath("/uploads"), pic);
                file.SaveAs(path);

                Bitmap img = new Bitmap(path);
                MemoryStream myStream = new MemoryStream();

                img.Save(myStream, ImageFormat.Jpeg);
                var storage = CloudConfigurationManager.GetSetting("myAzureStorage");
                CloudStorageAccount myAcct = CloudStorageAccount.Parse(storage);
                CloudBlobClient myClient = myAcct.CreateCloudBlobClient();
                CloudBlobContainer myContainer = myClient.GetContainerReference("uploads");

                myContainer.CreateIfNotExists();

                var myPermissions = myContainer.GetPermissions();
                if (myPermissions.PublicAccess == BlobContainerPublicAccessType.Off)
                {
                    myPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    myContainer.SetPermissions(myPermissions);
                }

                var imgBlob = myContainer.GetBlockBlobReference(pic); //, new DateTimeOffset(new DateTime(100000000)));
                imgBlob.Properties.ContentType = "image//jpeg";

                byte[] myBytes = myStream.GetBuffer();

                imgBlob.UploadFromByteArray(myBytes, 0, myBytes.Length);

                imageBlob = imgBlob.Uri.ToString();
                return imageBlob;
            }
            return "";
        }
        // GET: Guests/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GuestBookEntry guestBookEntry = db.GuestBookEntries.Find(id);
            if (guestBookEntry == null)
            {
                return HttpNotFound();
            }
            return View(guestBookEntry);
        }


        // POST: Guests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Message,GuestName,PhotoUrl,ThumbUrl,PartitionKey,RowKey,Timestamp,ETag")] GuestBookEntry guestBookEntry, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                    guestBookEntry.PhotoUrl = UploadBlob(file);

                db.Entry(guestBookEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guestBookEntry);
        }


        // GET: Guests/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GuestBookEntry guestBookEntry = db.GuestBookEntries.Find(id);
            if (guestBookEntry == null)
            {
                return HttpNotFound();
            }
            return View(guestBookEntry);
        }

        // POST: Guests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GuestBookEntry guestBookEntry = db.GuestBookEntries.Find(id);
            db.GuestBookEntries.Remove(guestBookEntry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
