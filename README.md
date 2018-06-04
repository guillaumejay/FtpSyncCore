# FtpSyncCore
A program to sync between two ftp (ftp/sftp protocol). Written in .NetCore / NetStandard for a smmal freelancing project.

Had fun with the strategy design patterns (nobody talks about design pattern anymore) and interface.

- FTPSync.Logic : a .Net Standard library
- FTPSync :  a console application, written in .Net Core (so works on Ubuntu too !)
- FTPSync.Service : a Windows Service
- FTPSync.Tests : some basic tests to check settings integrity
