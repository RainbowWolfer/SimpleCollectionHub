namespace SimpleCollectionHub.Framework.Resources;

public static class StaticStrings
{
	public static readonly string FileFilter_JsonFile = "JSON 文件|*.json";
	public static readonly string FileFilter_ZipFile = "ZIP 压缩文件|*.zip";
	public static readonly string FileFilter_CsvFile = "CSV 表格文件|*.csv";
	public static readonly string FileFilter_TsvFile = "TSV 表格文件|*.tsv";
	public static readonly string FileFilter_ExcelFile = "Excel 文件|*.xlsx;*.xlsb;*.xls";
	public static readonly string FileFilter_XlsxFile = "Excel 工作簿|*.xlsx";
	public static readonly string FileFilter_XlsbFile = "Excel 二进制工作簿|*.xlsb";

	public static readonly string FileFilter_TableFile_Import =
		"表格文件|*.csv;*.tsv;*.xlsx;*.xlsb;*.xls|" +
		"CSV 表格文件|*.csv|" +
		"TSV 表格文件|*.tsv|" +
		"Excel 文件|*.xlsx;*.xlsb;*.xls";

	public static readonly string FileFilter_TableFile_Export =
		"CSV 表格文件|*.csv|" +
		"TSV 表格文件|*.tsv|" +
		"Excel 工作簿|*.xlsx";

	public static readonly string FileFilter_TextFile = "文本文件|*.txt";
	public static readonly string FileFilter_PdfFile = "PDF 文件|*.pdf";
	public static readonly string FileFilter_WordFile = "Word 文档|*.docx;*.doc";
	public static readonly string FileFilter_ImageFile = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.ico";
	public static readonly string FileFilter_AllFile = "所有文件|*.*";
}