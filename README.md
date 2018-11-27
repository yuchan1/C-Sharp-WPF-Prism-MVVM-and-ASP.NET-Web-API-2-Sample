## C# WPF Application(Prism, MVVM pattern) + ASP.NET Web API 2, Sample project.

C# WPFアプリケーション(Prism, MVVM パターン) + ASP.NET Web API 2 のサンプルプロジェクトです。<br />
<br />
WPFアプリケーション(Prism MVVM)のViewModelから、HttpClientでHTTP通信(JSON形式のデータ)を利用して、<br />
ASP.NET Web API 2のコントローラーを実行します。RESTfulなサービスを提供する構成となっています。<br />
<br />
<br />
### 【全体図】

![Sample](README_img/wpf_webapi_image.png)
<br />
<br />
Visual Studio Express 2012(C#5.0)で実行可能です。<br />
サンプルプログラム自体は、マスタデータを取得・追加・更新・削除するシンプルな構成となっています。<br />
実際の業務系システムの開発現場では単純なマスタデータの更新処理だけではありませんが、<br />
このサンプルプログラムを利用して応用して作成していくことはできると思います。<br />
<br />
<br />
### 【画面イメージ】

![Sample](README_img/wpf_client_image.png)
<br />
<br />
### 【アプリケーション仕様】

①　クライアント<br />
言語・アプリケーション　：　C#、WPFアプリケーション (Visual Studio Express 2012 for Windows Desktop)<br />
主要パッケージ　　　　　：　Prism 6 (6.2), Prism 6 for Wpf (6.2), Unity for Prism 6 (6.2), Unity (4.0.1)<br />
　　　　　　　　　　　　　　CommonServiceLocator (1.3)<br />
　　　　　　　　　　　　　　Microsoft ASP.NET Web API 2.2 Client Libraries (5.2.6), Json.NET (9.0.1)<br />
<br />
②, ③　サーバー<br>
言語・アプリケーション　：　C#、ASP.NET Web API 2(Visual Studio Express 2012 for Web)<br />
主要パッケージ　　　　　：　Microsoft ASP.NET Web API 2.2 関係 (5.2.6), Json.NET (9.0.1)<br />
　　　　　　　　　　　　　　ADO.NET Entity Framework (6.2)<br />
データベース　　　　　　：　SQL Server Express 2012<br />
<br />
それぞれ2012版　に限定しているのは、オフラインインストールできる最新バージョンが2012までのため。<br />
2013版以降であれば、WPFアプリと同様のXaml形式であるUWPアプリ, Xamarin FormsなどもPrismでそれぞれ準備されているので選択できそうです。<br />
(Windowsフォームアプリは過去の技術)<br />
<br />
<br />
### 【WPFアプリケーションのPrismで利用している主な機能】

・ViewModelLocator(View-ViewModel関連付け)<br />
・ViewModel→Viewへのエラー通知機能(BindableBaseを継承したAddBindableBaseに追加)<br />
・Region + Navigation(画面遷移)<br />
・Delegate command(デリゲートコマンド)<br />
・Notification, Confirmation(ダイアログ)<br />
・PropertyChange(画面へ反映)<br />
・ViewDiscovery(起動時にログイン画面を表示)<br />
・BootStrapper<br />
※サンプルプログラムにはカスタムダイアログ(モーダルウィンドウ)を省略してます。別途追加予定。<br />
<br />
サンプルソースのWPFアプリケーションで使用しているPrism Wpfにより、<br />
Prismを利用したことがない人でも、基本的な利用を始めやすいと思います。<br />
<br />
Prism Wpf公式サンプル(英語)　https://github.com/PrismLibrary/Prism-Samples-Wpf<br />
Prism Wpf公式サンプル日本語説明　https://qiita.com/yuchan01/items/7b43a4cef5a91cf7a476<br />
<br />
<br />
<br />
### 【ASP.NET Web API 2 with Entity Framework】

Microsoft公式ドキュメント「Entity Framework 6 で Web API 2 を使用」にある、サンプルコードに準じたものです。<br />
また、認証処理ではForms認証を利用してます。<br />
<br />
最初にプロジェクト実行時にMainWindowViewModelで、CookieContainerとHttpClientを準備します。<br />
WPFアプリケーションのログイン画面でログイン要求して  →　ASP.NET Web API 2のAuthControllerで認証、レスポンスから、ユーザー名と権限IDをWPF側のMainWindowViewModelのプロパティに格納します。<br />
<br />
ASP.NET Web API 2で作成したEntityFrameworkで使用するPoco Modelは、WPFアプリケーション側にコピーして使用してます。<br />
<br />
<br />
### 【実行方法】

1、SQL Server Express 2012で好きな名前のデータベースを作成する。<br />
　　※EntityFrameworkでORMを使用してしますのでデータベース種類は問わないかもしれません。<br />
  　(Oracle, PostgreSQL, ...)<br />
<br>
2、Visual Studio Express 2012 for WebでASP.NET Web API 2のソリューションを起動して、<br />
　　「プロジェクト　→　プロパティ　→　設定」でDebugConnectionString、ReleaseConnectionStringにデータベース接続方法を設定する。 <br />
  　上記で作成したデータベースへテスト接続OKのこと。<br />
　　パッケージマネージャのコンソールでEntityFrameworkのマイグレーション有効化、追加、<br />
　　データベースアップデートを実行してください。<br />
<br>
3、SQL Server Management Studioで適当なデータを入力してください。<br />
　　2つのテーブルがありますので、それぞれ1行追加でOKです。<br />
　　M_Authorities　→ M‗Members の順に追加します。<br />
　　※そのうちテストデータを用意するかもしれません。<br />
<br>
4、Visual Studio Express 2012 for Web　で開発用サーバーを指定してますので、<br />
　　そのままプログラムを実行して下さい。<br />
<br>
5、Visual Studio Express 2012 for Windows Desktopのソリューションを起動して、<br />
    MainWindowViewModelに書いてあるlocalhostのポート番号が合っているか確認して下さい。    
　　プログラム実行するとログイン画面が表示されます。<br />
   <br />
<br />
<br />
### 【サンプルプログラムの既存の問題】

・ログインパスワード暗号化とSSL化(HTTPS利用)しておきたい。Expressでは開発用サーバーにSSL使えない？<br />
・ログイン要求で入手したMemberName、AuthorityIDの格納場所は、MainWindowViewModelのstaticプロパティで良いのか？<br />
・httpサーバーのアドレスを、MainWindowViewModelのコンストラクタ内にベタ書きしてるので設定ファイルに移したほうが良いかな。<br />
・ASP.NET Web APIにasyncを使っている理由はよくわかってないです。(Microsoft公式ドキュメントのサンプルが使っているから)<br />
・他にも何か改善点があるかもしれません。<br />
・テーブルの命名規則の修正が必要<br />
Flag → IsDeleting、Create → CreatedAt (追加必要)、Update → UpdatedAt<br />
・AddDbContext.csで、作成日時の更新処理の追加が必要<br />
・AddDbContext.csで、decimalの小数点第3以下も使えるようにする<br />
protected override void OnModelCreating(DbModelBuilder modelBuilder) {<br />
    modelBuilder.Conventions.Remove<DecimalPropertyConvention>();<br />
    modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 4));<br />
}<br />
<br />
<br />
※IISに乗せて動作しない時は以下の記事も参考にしてください。<br />
IIS7.5にASP.NET Web API 2を載せる方法と、HTTP Put, Deleteで失敗するの回避策<br />
https://qiita.com/yuchan01/items/32dfc83d8b7eab6c5dae<br />
