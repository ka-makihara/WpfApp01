#-*- coding:utf-8 -*-

#
# FTP Utility
#

import os
from ftplib import FTP, error_perm
import glob
#from typing import Generator, List

FTP_user = 'makihara'
FTP_pass = 'wildgeese'

def dbg_print(msg):
	print msg 

def connectFTP(host):
	ftp = FTP(host, user=FTP_user, passwd=FTP_pass)

	return ftp

def get_file(ftp, remote_path, local_path):
	with open(local_path,'wb') as f:
		ftp.retrbinary('RETR {}'.format(remote_path),f.write)


def rename(ftp, fromname, toname):
	ftp.rename(fromname,toname)

def mkdir(ftp, path):
	try:
		ftp.mkd(path)
		dbg_print('mkdir({0}'.format(path))

	except error_perm as e:
		#フォルダが存在しているとエラーとなるので、無視する
		dbg_print('{}{}'.format(e.args,path))
		pass

def rm_dir(ftp, path):
	'''
		FTP下の指定されたフォルダを削除する
		ファイルが含まれる場合はファイルも削除する
		Parameters:
		-----------
			ftp: FTP Object
			path: FTP下のパス
		returns
		-----------
			None
	'''
	items = ftp.mlsd(path)

	for filename, opt in items:
		if opt['type'] == 'cdir' or opt['type'] == 'pdir':
			# '.' or '..' 
			continue
		elif opt['type'] == 'dir':
			#フォルダの場合はそのフォルダを削除(再起呼び出し)
			rm_dir(ftp, path + '/' + filename)
		else:
			try:
				ftp.delete(path + '/' + filename)
				dbg_print('delete({})'.format(path+'/'+filename))
			except error_perm as e:
				dbg_print('{}:{}'.format(e.args,path + '/' + filename))
				pass

	#最後に指定されたフォルダを消去
	try:
		ftp.rmd(path)
		dbg_print('rmdir({})'.format(path))
	except error_perm as e:
		pass
		dbg_print('{}:{}'.format(e.args,path))

def upload(ftp, local_path, remote):
	#転送先のフォルダを作る
	mkdir(ftp,remote.replace('\\','/'))

	#ファイル一覧の作成
	files = glob.glob(local_path+'\\*')

	dbg_print('c={}'.format(len(files)))
	for file in files:
		dbg_print('target={}\n'.format(file))
		if os.path.isdir(file):
			#フォルダなら
			path = file.split('\\')
			upload(ftp,file,remote+'/'+path[-1])

		else:
			#ファイルなら
			items = file.split('\\')
			tgtFile = remote.replace('\\','/') + '/' + items[-1]
			dbg_print('STOR {0} from {1}'.format(tgtFile,file))
			try:
				with open(file,'rb') as f:
					ftp.storbinary('STOR {}'.format(tgtFile),f)
					#ftp.storlines('STOR {}'.format(tgtFile),f)
					dbg_print('STOR end')
			except error_perm as e:
				dbg_print('{}:{}'.format(e.args,tgtFile))

def main():
	ftp = connectFTP('localhost')
	#ftp.set_pasv('true')

	upload(ftp,'C:\\Users\\ka.makihara\\Desktop\\update_test\\temp','\\Fuji_tmp')
	#rm_dir(ftp,'Fuji_tmp')
	#cur = os.getcwd()
	#get_file(ftp,'/Fuji/System3/Program/Peripheral/UpdateCommon.inf','common.inf')

	ftp.close()

if __name__ == '__main__':
	main()