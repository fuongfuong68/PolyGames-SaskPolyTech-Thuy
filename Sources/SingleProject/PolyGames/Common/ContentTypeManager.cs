
namespace PolyGames.Common
{
    public class ContentTypeManager
    {
        public ContentTypeManager()
        {

        }

        public bool IsImage(string contentType)
        {
            if (contentType == "image/jpeg")
                return true;

            if (contentType == "image/png")
                return true;

            return false;
        }

        public bool IsVideo(string contentType)
        {
            if (contentType == "video/mp4")
                return true;

            if (contentType == "video/x-msvideo")
                return true;

            return false;
        }

        public bool IsZip(string contentType)
        {
            if (contentType == "application/zip")
                return true;

            if (contentType == "application/x-zip-compressed")
                return true;

            if (contentType == "multipart/x-zip")
                return true;

            return false;
        }
    }
}