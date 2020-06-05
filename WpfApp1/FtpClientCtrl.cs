using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using FluentFTP;

namespace WpfApp1
{
    class FtpClientCtrl
    {
        private FtpClient client = null;


        public bool Connect(String host, String user, String pass)
        {
           if( client == null) {
                client = new FtpClient();
                client.Host = host;
                client.Credentials = new NetworkCredential( user, pass );

                return true;
           }
           else {
                return true;
           }
        }

        public bool Close()
        {
            if( client != null) {
                client.Disconnect();
            }
            return true;
        }
        public bool Upload(String localPath, String remotePath)
        {
            if( client == null) {
                return false;
            }
            FtpStatus status = client.UploadFile( localPath, remotePath, FtpRemoteExists.Overwrite, true,FtpVerify.None );

            if( status.IsFailure() ) {

            }

            return status.IsSuccess();
        }

        public bool UploadDirectory(String localPath, String remotePath)
        {
            if( client == null) {
                return false;
            }
            List<FtpResult> status = client.UploadDirectory( localPath, remotePath,FtpFolderSyncMode.Update,FtpRemoteExists.Overwrite );

            return true;
        }
    }
}
