# LambdaReportsApp

## 概要
LambdaReportsAppは、AWS Lambdaを使用してHTTPリクエストを処理し、ActiveReports for .NETを用いてPDFを生成するアプリケーションです。このアプリケーションは、指定されたデータを基にレポートを作成し、PDF形式で出力します。
ソースコードはブログ記事「[AWS LambdaとActiveReports for .NETでつくる帳票生成API](https://devlog.mescius.jp/activereports-aws-lambda/)」内で解説している内容となります。

## 使用技術
- **AWS Lambda**: サーバーレスコンピューティングサービスを利用して、HTTPリクエストを処理します。
- **MESCIUS ActiveReports**: レポートの生成とPDF出力に使用します。
- **.NET**: アプリケーションの開発に使用されています。

## セットアップ手順
1. 必要な.NET SDKをインストールします。
2. プロジェクトをクローンまたはダウンロードします。
3. 必要なNuGetパッケージを復元します。

## 使用方法
1. AWS Lambdaをデプロイします。
2. HTTPリクエストを送信し、`DataJson`パラメータを指定します。
3. レスポンスとして生成されたPDFを受け取ります。

## フォント
このアプリケーションでは、`ipag.ttf`フォントが埋め込まれて使用されています。
