// NSwag Swagger UIのPDFダウンロードボタン実装
(function () {
    // ページの読み込み後に実行
    window.addEventListener('load', function () {
        console.log("PDF Download Button Handler Loaded");

        // MutationObserverの設定
        setupResponseObserver();
    });

    // レスポンスの変更を監視
    function setupResponseObserver() {
        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                // 新しく追加されたノードがある場合
                if (mutation.addedNodes && mutation.addedNodes.length > 0) {
                    // レスポンスボディを検索
                    checkResponsesForPdf();
                }
            });
        });

        // ドキュメント全体を監視
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });

        // 実行ボタンのクリックイベントも監視
        document.addEventListener('click', function (e) {
            // 実行ボタンのクリック後にチェック
            if (e.target.classList.contains('nswag-execute-button') ||
                e.target.closest('.nswag-execute-button')) {
                setTimeout(checkResponsesForPdf, 1000);
            }
        });
    }

    // レスポンスボディをチェックしてPDF判定
    function checkResponsesForPdf() {
        // NSwagのレスポンスコンテナを取得
        const responseElements = document.querySelectorAll('.responses-table.live-responses-table pre');

        responseElements.forEach(element => {

            console.log("Checking response element:", element);
            // 既に処理済みの要素はスキップ
            if (element.dataset.pdfChecked === 'true') return;

            console.log("Checking response element:", element);
            // 処理済みとしてマーク
            element.dataset.pdfChecked = 'true';

            try {
                const responseText = element.textContent;
                if (!responseText) return;

                console.log("Response text:", responseText);
                const responseData = JSON.parse(responseText);
                console.log("Parsed response data:", responseData);

                // PDF応答かどうかチェック - プロパティ名の大文字小文字を両方チェック
                if (responseData) {
                    // statusCode/StatusCode チェック
                    const hasValidStatus = responseData.statusCode === 200 || responseData.StatusCode === 200;
                    
                    // headers/Headers チェック
                    const headers = responseData.headers || responseData.Headers || {};
                    const hasValidContentType = headers["Content-Type"] === "application/pdf" || 
                                               headers["content-type"] === "application/pdf";
                    
                    // isBase64Encoded/IsBase64Encoded チェック
                    const isBase64 = responseData.isBase64Encoded === true || responseData.IsBase64Encoded === true;
                    
                    // body/Body チェック
                    const hasBody = responseData.body || responseData.Body;

                    console.log("Status check:", hasValidStatus);
                    console.log("Content-Type check:", hasValidContentType);
                    console.log("Base64 check:", isBase64);
                    console.log("Body check:", !!hasBody);

                    if (hasValidStatus && hasValidContentType && isBase64 && hasBody) {
                        console.log("PDF response detected - adding download button");

                        // ボタンを作成して追加
                        addPdfDownloadButton(element, responseData);
                    }
                }
            } catch (e) {
                console.log("Not a valid JSON or not a PDF response", e);
            }
        });
    }

    // ダウンロードボタンを追加
    function addPdfDownloadButton(element, responseData) {
        // ボタンコンテナを作成
        const buttonContainer = document.createElement('div');
        buttonContainer.style.marginTop = '10px';

        // ダウンロードボタンを作成
        const downloadButton = document.createElement('button');
        downloadButton.className = 'pdf-download-button';
        downloadButton.textContent = 'Download PDF';
        downloadButton.onclick = function () {
            downloadPdf(responseData);
        };

        // ボタンを追加
        buttonContainer.appendChild(downloadButton);

        // レスポンス要素の後にボタンを挿入
        if (element.parentNode) {
            element.parentNode.insertBefore(buttonContainer, element.nextSibling);
        }
    }

    // PDF実際のダウンロード処理
    function downloadPdf(responseData) {
        try {
            // body または Body プロパティを取得
            const base64Content = responseData.body || responseData.Body;
            if (!base64Content) {
                throw new Error("PDF content not found in response");
            }

            // Base64文字列をデコード
            const binaryString = atob(base64Content);
            const bytes = new Uint8Array(binaryString.length);
            for (let i = 0; i < binaryString.length; i++) {
                bytes[i] = binaryString.charCodeAt(i);
            }

            // PDFのBlobを作成
            const blob = new Blob([bytes.buffer], { type: 'application/pdf' });

            // ファイル名を取得
            let filename = "report.pdf";  // デフォルト名
            const headers = responseData.headers || responseData.Headers || {};
            const contentDisposition = headers["Content-Disposition"] || headers["content-disposition"];
            
            if (contentDisposition) {
                const match = contentDisposition.match(/filename="([^"]+)"/);
                if (match && match[1]) {
                    filename = match[1];
                }
            }

            // ダウンロード用リンクを作成
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();

            // クリーンアップ
            setTimeout(function () {
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }, 100);

            console.log("PDF download initiated for file:", filename);
        } catch (e) {
            console.error("PDF download failed:", e);
            alert("Failed to download PDF: " + e.message);
        }
    }
})();
