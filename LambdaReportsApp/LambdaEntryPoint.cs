namespace LambdaReportsApp;

/// <summary>
/// このクラスはAPIGatewayProxyFunctionから拡張されており、実際のLambda関数のエントリーポイントである
/// FunctionHandlerAsyncメソッドを含んでいます。Lambdaハンドラフィールドは以下のように設定する必要があります。
/// 
/// LambdaReportsApp::LambdaReportsApp.LambdaEntryPoint::FunctionHandlerAsync
/// </summary>
public class LambdaEntryPoint :

    // ベースクラスはLambda関数を呼び出すAWSサービスに一致するように設定する必要があります。そうしないと、
    // Amazon.Lambda.AspNetCoreServerは受信リクエストを正しく変換して有効なASP.NET Coreリクエストにすることができません。
    //
    // API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    // API Gateway HTTP APIペイロードバージョン1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    // API Gateway HTTP APIペイロードバージョン2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
    // アプリケーションロードバランサー                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
    // 
    // 注: AWS::Serverless::Functionリソースをイベントタイプ「HttpApi」で使用する場合、ペイロードバージョン2.0がデフォルトとなり、
    // Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunctionをベースクラスにする必要があります。

    Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
{
    /// <summary>
    /// ビルダーには設定、ロギング、およびAmazon API Gatewayが既に構成されています。このメソッドでUseStartup<>()メソッドを使用して
    /// スタートアップクラスを構成する必要があります。
    /// </summary>
    /// <param name="builder">構成するIWebHostBuilder。</param>
    protected override void Init(IWebHostBuilder builder)
    {
        builder
            .UseStartup<Startup>();
    }

    /// <summary>
    /// このオーバーライドを使用して、IHostBuilderに登録されているサービスをカスタマイズします。
    /// 
    /// このメソッド内でIWebHostBuilderを構成するためにConfigureWebHostDefaultsを呼び出さないことをお勧めします。
    /// 代わりに、Init(IWebHostBuilder)オーバーロードでIWebHostBuilderをカスタマイズします。
    /// </summary>
    /// <param name="builder">構成するIHostBuilder。</param>
    protected override void Init(IHostBuilder builder)
    {
    }
}