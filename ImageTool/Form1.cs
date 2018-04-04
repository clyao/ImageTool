using System;
using System.IO;
using System.Windows.Forms;

namespace ImageTool
{
    public partial class Form1 : Form
    {

        private string filePath = null;
        private string fileName = null;
        private string fileExtension = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.doc,*.docx,*.xls,*.xlsx,*.ppt,*.pptx,*.pdf)|*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.pdf";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = fileDialog.FileName.ToString();
                txtFilePath.Text = filePath;
                fileName = Path.GetFileName(filePath);
                fileExtension = Path.GetExtension(filePath);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (filePath != null)
            {
                if (".doc" == fileExtension || "*.docx" == fileExtension)
                {
                    lblConver.Visible = true;
                    lblConver.Refresh();
                    wordConverToImage(filePath, fileName);
                }
                else if (".ppt" == fileExtension || ".pptx" == fileExtension)
                {
                    lblConver.Visible = true;
                    lblConver.Refresh();
                    pptConverToImage(filePath, fileName);
                }
                else if (".pdf" == fileExtension)
                {
                    lblConver.Visible = true;
                    lblConver.Refresh();
                    pdfConverToImage(filePath, fileName);
                }
                else
                {
                    MessageBox.Show("暂时只支持word、ppt、pdf转换", "提示");
                }
            }
            else
            {
                MessageBox.Show("请选择文件", "转换结果");
            }

        }

        /// <summary>
        /// 将word文档转换为jpg图片
        /// </summary>
        /// <param name="filePath">选择文件的路径</param>
        /// <param name="fileName">选择文件的文件名</param>
        private void wordConverToImage(string filePath, string fileName)
        {
            try
            {
                Aspose.Words.Document document = new Aspose.Words.Document(filePath);
                Aspose.Words.Saving.ImageSaveOptions imageSaveOptions = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Jpeg);
                imageSaveOptions.Resolution = 300;
                for (int i = 0; i < document.PageCount; i++)
                {
                    document.Save(fileName.Split('.')[0] + ".jpg", imageSaveOptions);
                }
                lblConver.Visible = false;
                DialogResult dialogResult = MessageBox.Show("转换成功,是否打开转换图片文件夹", "转换结果", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    System.Diagnostics.Process.Start("Explorer.exe", Environment.CurrentDirectory);
                }
            }
            catch (Exception)
            {
                lblConver.Visible = false;
                MessageBox.Show("转换失败", "转换结果");
            }
        }

        /// <summary>
        /// 将pdf文档转换为jpg图片
        /// </summary>
        /// <param name="filePath">选择文件的路径</param>
        /// <param name="fileName">选择文件的文件名称</param>
        private void pdfConverToImage(string filePath, string fileName)
        {
            try
            {
                Aspose.Pdf.Document document = new Aspose.Pdf.Document(filePath);
                Aspose.Pdf.Devices.Resolution resolution = new Aspose.Pdf.Devices.Resolution(300);
                Aspose.Pdf.Devices.JpegDevice jpegDevice = new Aspose.Pdf.Devices.JpegDevice(resolution, 100);
                for (int i = 1; i <= document.Pages.Count; i++)
                {
                    FileStream fileStream = new FileStream(fileName.Split('.')[0] + i + ".jpg", FileMode.OpenOrCreate);
                    jpegDevice.Process(document.Pages[i], fileStream);
                    fileStream.Close();
                }
                lblConver.Visible = false;
                DialogResult dialogResult = MessageBox.Show("转换成功,是否打开转换图片文件夹", "转换结果", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    System.Diagnostics.Process.Start("Explorer.exe", Environment.CurrentDirectory);
                }
            }
            catch (Exception)
            {
                lblConver.Enabled = false;
                MessageBox.Show("转换失败", "转换结果");
            }
        }

        /// <summary>
        /// 将ppt文件转换为图片
        /// </summary>
        /// <param name="filePath">选择文件的路径</param>
        /// <param name="fileName">选择文件的名称</param>
        private void pptConverToImage(string filePath, string fileName)
        {
            try
            {
                Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(filePath);
                string tempPath = Environment.CurrentDirectory + fileName.Split('.')[0] + ".pdf";
                //先将ppt转为pdf
                presentation.Save(tempPath, Aspose.Slides.Export.SaveFormat.Pdf);
                pdfConverToImage(tempPath, fileName);
                File.Delete(tempPath);
            }
            catch (Exception)
            {

                lblConver.Enabled = false;
                MessageBox.Show("转换失败", "转换结果");
            }
        }
    }
}
