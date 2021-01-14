
# AX.SimpleOCR

基于 .Net 5.0 开发的基于百度 API 的截图 OCR 识别工具 

### 背景

日常做笔记总会有图片和视频中的大量文字需要记录，
手打过于浪费时间，所以设计此小工具使用。

### 功能

目前仅支持截图后调用百度 OCR 接口识别。

### 使用方式

安装 Net5.0 桌面运行时 https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-5.0.0-windows-x64-installer
首次启动会自动初始化配置文件，提示未配置百度相关密钥。打开配置文件编辑后关闭即可。

### 配置文件

程序所在目录下 setting.json 文件

- ApiKey						百度接口key
- SecretKey						百度接口密钥
- Timeout						接口请求超时时间 默认为 60000
- OnScreenshotVisibleForm		截图时隐藏主窗体 默认为 true

### 更新记录

- 2021.01.13

发布 V1.0

- 2021.01.14

优化配置文件默认生成内容
增加程序图标，任务栏不显示
增加全局快捷键  Ctrl+Q

- 2021.01.15

优化双击任务栏图标打开后窗体获得焦点
优化无任务栏项后的最小化窗体显示
