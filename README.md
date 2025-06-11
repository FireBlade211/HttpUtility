# HTTP Utility
A WinUI 3 application that allows you to send HTTP requests.

# Table of Contents
[View full](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#full-table-of-contents)
- [Installation](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#installation)
- - [Certificate error (0x800B010A)](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#certificate-error-0x800b010a)
- [Dependencies](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#dependencies)
- [License](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#license)
- [Contributions](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#contributions)
- [How to Use](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#how-to-use)
- - [Options](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#options)
- - [Response](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#response)
- - [Send Request](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#send-request)
- - [Settings](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#settings)

# Installation
Download the latest release from the [Releases](https://github.com/FireBlade211/HttpUtility/releases) page, download the **ZIP file** for your architecture (e.g, "HTTP Utility_1.0.0.0_x64.zip"), unzip it, and run the MSIX file.

## Certificate error (*0x800B010A*)
If you get a certificate error (**0x800B010A**) after running the MSIX file, close out of the installer and follow the steps below.
1. Go back to the folder you unzipped.
2. Find the *.cer* file and double-click on it.
3. In the dialog that shows up, click **Install Certificate**.
4. In the **Certificate Import Wizard**, select *Local Machine* and press Next.
5. Authenticate with **User Account Control** to continue.
6. On the next page, select **Place all certificates in the following store**, and in the *Certificate store* box, click Browse, find **Trusted Root Certification Authorities**, and press OK. Then, press Next.
7. Finally, hit Finish, close the message box that shows up, and close out of the **Certificate** dialog.
8. Now, you can run the **MSIX** file again, and, if everything was done correctly, you should be able to install the program.

# Dependencies
This app depends on the **Windows App SDK**, but modern versions of Windows should include it by default.

# License
**HTTP Utility** is open-source and available under the **MIT License**. See the [LICENSE](LICENSE) file for more details.

# Contributions
**HTTP Utility** is free and open-source. Contributions are welcome!

# How to Use
After running **HTTP Utility**, you can configure the options used to send the HTTP request in the **Options** tab, view the sent response in the **Response** tab, send the HTTP request by clicking the **Send Request** button, and configure settings in the **Settings** tab. The specific tabs and buttons, and their usage, can be found below:

## Options
Allows you to configure the options for the HTTP request. Here are the settings and their explanation:

### URL
The URL to ping.
### Basic settings
- **Request Type** - The method to send the HTTP request with. Options include: **GET, POST, PUT, PATCH, DELETE**.
- **Headers** - HTTP headers to send along with the request. These headers can't have duplicate or empty keys or empty values. Trying to avoid these rules in the header editor will make the OK button grayed out.
- **Body** - The body content of the HTTP request.
### Export and import
The options here aren't standalone *request settings*, but rather buttons to perform actions.
- **Export** - Export the current request options to a file. This file will be saved with **HTTP Utility**'s custom *.hro (HTTP Request Options)* file format.
- **Import** - Import request options from a *.hro** file. (*see above)
-----------
For a quicker explanation, you can simply look at the **description** under each option card without having to look at this README.
## Response
View the response sent back by the server requested. By default, this will be empty. To send the request and be able to view this tab properly, use the **Send Request** button. Once a response is sent back, you can switch to one of two sub-tabs:

### Raw
View the raw response sent in plain text.

### Render
Render the sent back data in a browser and view it.

## Send Request
This button, once pressed, will send the HTTP request and show a message box showing if it was successful. If it is, the response can be viewed in the **Response** tab. If it's not, the message box will tell you the HTTP error code and message that occurred.

## Settings
This tab allows you to configure app settings, which include:

### Appearance & behavior
- **App theme** - Choose the theme that should be used by **HTTP Utility**. *Use system theme* will follow your Windows theme.
- **Navigation style** - Choose a navigation style to use. Options: **Left, Left (overlay), Left (compact), Top**. The default is **Top**.
#### Sound
Enable or disable sound effects.
- **Volume** - Configure the volume of the sound effects.
- **Enable Spatial Audio** - Enable or disable spatial audio for a more immersive experience.
- *Test audio button* - As the text on the button says, you can click on this button to play a random sound effect.
----
- **Window background style** - Pick a window background style. **Acrylic (thin)** looks the nicest, but it also may be glitchy, so use at your own risk.
### About
View information about **HTTP Utility**, such as your version. This will automatically show a special card on **Debug** builds.

# Full Table of Contents
[Go to Top](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#http-utility)
[View compact](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#table-of-contents)
- [Installation](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#installation)
- - [Certificate error (0x800B010A)](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#certificate-error-0x800b010a)
- [Dependencies](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#dependencies)
- [License](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#license)
- [Contributions](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#contributions)
- [How to Use](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#how-to-use)
- - [Options](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#options)
- - - [URL](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#url)
- - - [Basic settings](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#basic-settings)
- - - [Export and import](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#export-and-import)
- - [Response](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#response)
- - - [Raw](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#raw)
- - - [Render](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#render)
- - [Send Request](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#send-request)
- - [Settings](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#settings)
- - - [Appearance & behavior](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#appearance--behavior)
- - - - [Sound](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#sound)
- - - [About](https://github.com/FireBlade211/HttpUtility/blob/main/README.md#about)
