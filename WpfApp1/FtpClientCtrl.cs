using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using FluentFTP;

namespace WpfApp1
{
    //'public' class にしないとメンバー関数にアクセスできない

    public class FtpClientCtrl
    {
        private  FtpClient client = null;


        public bool ConnectFTP(string host, string user, string pass)
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
        public bool MakeDirectory(String path)
        {
            if (client != null){
                return client.CreateDirectory(path);
            }
            return false;
        }
        public bool Rename(String fromPath, String toPath)
        {
            if( client != null){
                client.Rename(fromPath, toPath);
                return true;
            }
            return false;
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
        public bool RemoveDirectory(String remotePath)
        {
            if( client == null)
            {
                return false;
            }

            client.DeleteDirectory(remotePath,FtpListOption.AllFiles | FtpListOption.Recursive);

            return true;
        }
    }
}
