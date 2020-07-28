# winForm_keyboardGlobalCatch 失敗!

## 原先需求: 在windows鎖屏時 自動側錄鍵盤密碼 並由enter鍵觸發寄信功能

## 失敗原因: 在windows鎖屏時, 因為安全性不會發送滑鼠指令與鍵盤操作指令給執行中程序,使得註冊的windowsHook抓不到鍵盤指令而失敗

reference link :　https://stackoverflow.com/questions/1583326/hook-a-hotkey-from-windows-logon-screen
