using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using GrapeCity.ActiveReports.Export.Pdf.Page;
using GrapeCity.ActiveReports.Extensibility.Rendering.IO;
using GrapeCity.ActiveReports.Rendering.IO;
using GrapeCity.ActiveReports;
using System.Xml;
using System.Reflection;
using static GrapeCity.Enterprise.Data.DataEngine.DataProcessing.DataProcessor;
using System.Text.Json;
using System.Text.Json.Serialization;
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaReportsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfExportController : ControllerBase
    {
        /// <summary>
        /// GETメソッド
        /// </summary>
        /// <param name="DataJson">レポートデータ</param>
        /// <returns>APIGatewayProxyResponse</returns>
        [HttpGet]
        public APIGatewayProxyResponse Get([FromQuery] string DataJson)
        {
            return ExportFunction(DataJson);
        }

        /// <summary>
        /// POSTメソッド
        /// </summary>
        /// <param name="DataJson">レポートデータ</param>
        /// <returns>APIGatewayProxyResponse</returns>
        [HttpPost]
        public APIGatewayProxyResponse Post([FromBody] string DataJson)
        {
            return ExportFunction(DataJson);
        }

        /// <summary>
        /// レポート出力処理
        /// </summary>
        /// <param name="DataJson">レポートデータ</param>
        /// <returns>APIGatewayProxyResponse</returns>
        private APIGatewayProxyResponse ExportFunction(string DataJson)
        {
            //戻り値を設定
            APIGatewayProxyResponse result;


            try
            {
                MemoryStream pdfResult = GetResult(DataJson);
                string base64Pdf = ConvertStreamPdfToBase64(pdfResult);
                result = new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    IsBase64Encoded = true,
                    Body = base64Pdf,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/pdf" },
                        { "Content-Disposition", "attachment; filename=\"report.pdf\"" }
                    }
                };

            }
            catch (Exception ex)
            {
                result = new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    IsBase64Encoded = false,
                    Body = ex.Message,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "text/plain" }
                    }
                };
            }

            return result;
        }

        private static MemoryStream GetResult(string DataJson)
        {
            MemoryStream? outputStream = null;
            try
            {
                //アセンブリ取得
                var assembly = Assembly.GetExecutingAssembly();

                //レポートファイルのストリームを取得
                var stream = assembly.GetManifestResourceStream("LambdaReportsApp.Invoice_bluegray.rdlx");
                var streamReader = new StreamReader(stream);

                //レポートファイルを読み込み
                GrapeCity.ActiveReports.PageReport pageReport = new PageReport(streamReader);

                ////フォントの設定
                pageReport.FontResolver = new EmbeddedFontResolver();

                //レポートにパラメータを設定
                pageReport.Document.Parameters[0].Values[0].Value = DataJson;

                //PDF出力設定
                GrapeCity.ActiveReports.Export.Pdf.Page.Settings pdfSetting = new GrapeCity.ActiveReports.Export.Pdf.Page.Settings();
                GrapeCity.ActiveReports.Export.Pdf.Page.PdfRenderingExtension pdfRenderingExtension = new GrapeCity.ActiveReports.Export.Pdf.Page.PdfRenderingExtension();
                GrapeCity.ActiveReports.Rendering.IO.MemoryStreamProvider outputProvider = new GrapeCity.ActiveReports.Rendering.IO.MemoryStreamProvider();

                //PDFレンダリング
                pageReport.Document.Render(pdfRenderingExtension, outputProvider, pdfSetting);

                //PDFファイルをMemoryStreamに変換
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                outputProvider.GetPrimaryStream().OpenStream().CopyTo(ms);

                outputStream = ms;
            }
            catch (Exception)
            {
                //エラー処理 エラー通知を返す
                throw;
            }
            return outputStream ?? new MemoryStream();
        }

        private string ConvertStreamPdfToBase64(MemoryStream pdfStream)
        {
            return Convert.ToBase64String(pdfStream.ToArray());
        }

    }
}
