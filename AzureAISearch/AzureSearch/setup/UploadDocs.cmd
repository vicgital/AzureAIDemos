@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

rem Set values for your storage account
set subscription_id=015616dc-c49f-494f-b567-110aa5af7d50
set azure_storage_account=stazuresearchpoc1
set azure_storage_key=I78aLR6QGL1kGcak9iIwgfP1+7W6tLHQAq1jH8/9w6nLwn349zUxPvPkCDpq/exy2rJtiy6eEMyK+AStu2Tj9g==


echo Creating container...
call az storage container create --account-name !azure_storage_account! --subscription !subscription_id! --name margies --auth-mode key --account-key !azure_storage_key! --output none

echo Uploading files...
call az storage blob upload-batch -d margies -s data --account-name !azure_storage_account! --auth-mode key --account-key !azure_storage_key!  --output none
