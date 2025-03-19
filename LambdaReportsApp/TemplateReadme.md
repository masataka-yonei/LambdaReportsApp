# ASP.NET Core Web API サーバーレスアプリケーション

このプロジェクトは、ASP.NET Core Web API プロジェクトを AWS Lambda として Amazon API Gateway を通じて実行する方法を示しています。NuGet パッケージ [Amazon.Lambda.AspNetCoreServer](https://www.nuget.org/packages/Amazon.Lambda.AspNetCoreServer) には、API Gateway からのリクエストを ASP.NET Core フレームワークに変換し、ASP.NET Core からのレスポンスを API Gateway に戻すために使用される Lambda 関数が含まれています。

Amazon.Lambda.AspNetCoreServer パッケージの動作方法やその動作を拡張する方法については、GitHub の [README](https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.AspNetCoreServer/README.md) ファイルを参照してください。

### API Gateway HTTP API の設定 ###

API Gateway は、従来の REST API と新しい HTTP API をサポートしています。さらに、HTTP API は 2 つの異なるペイロード形式をサポートしています。2.0 形式を使用する場合、`LambdaEntryPoint` の基本クラスは `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction` でなければなりません。1.0 ペイロード形式の場合、基本クラスは REST API と同じ `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction` です。
**注:** `HttpApi` イベントタイプを持つ `AWS::Serverless::Function` CloudFormation リソースを使用する場合、デフォルトのペイロード形式は 2.0 であるため、`LambdaEntryPoint` の基本クラスは `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction` でなければなりません。

### アプリケーションロードバランサーの設定 ###

このプロジェクトを API Gateway の代わりにアプリケーションロードバランサーからのリクエストを処理するように設定するには、`LambdaEntryPoint` の基本クラスを `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction` から `Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction` に変更します。

### プロジェクトファイル ###

* serverless.template - サーバーレス関数やその他の AWS リソースを宣言するための AWS CloudFormation サーバーレスアプリケーションモデルテンプレートファイル
* aws-lambda-tools-defaults.json - Visual Studio および AWS へのコマンドラインデプロイメントツールで使用するデフォルトの引数設定
* LambdaEntryPoint.cs - **Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction** を継承するクラス。このファイルのコードは ASP.NET Core ホスティングフレームワークをブートストラップします。Lambda 関数は基本クラスで定義されています。アプリケーションロードバランサーを使用する場合は、基本クラスを **Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction** に変更します。
* LocalEntryPoint.cs - ローカル開発用に、通常の ASP.NET Core アプリケーションと同様に Kestrel で ASP.NET Core ホスティングフレームワークをブートストラップする実行可能な Main 関数を含みます。
* Startup.cs - ASP.NET Core が使用するサービスを構成するために使用される通常の ASP.NET Core Startup クラス。
* appsettings.json - ローカル開発用。
* Controllers\ValuesController - 例としての Web API コントローラー

選択したオプションに応じて、テストプロジェクトも含まれる場合があります。

## Visual Studio からの手順:

サーバーレスアプリケーションをデプロイするには、ソリューションエクスプローラーでプロジェクトを右クリックし、*AWS Lambda に公開* を選択します。

デプロイされたアプリケーションを表示するには、AWS エクスプローラー ツリーの AWS CloudFormation ノードの下に表示されるスタック名をダブルクリックしてスタックビューウィンドウを開きます。スタックビューには、公開されたアプリケーションのルート URL も表示されます。

## コマンドラインから始めるための手順:

テンプレートとコードを編集したら、コマンドラインから [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) を使用してアプリケーションをデプロイできます。

Amazon.Lambda.Tools Global Tools がまだインストールされていない場合は、インストールします。
```
    dotnet tool install -g Amazon.Lambda.Tools
```

すでにインストールされている場合は、新しいバージョンが利用可能かどうかを確認します。
```
    dotnet tool update -g Amazon.Lambda.Tools
```

単体テストを実行します。
```
    cd "LambdaReportsApp/test/LambdaReportsApp.Tests"
    dotnet test
```

アプリケーションをデプロイします。
```
    cd "LambdaReportsApp/src/LambdaReportsApp"
    dotnet lambda deploy-serverless
```
