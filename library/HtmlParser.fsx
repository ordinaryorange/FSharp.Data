(**
// can't yet format YamlFrontmatter (["category: Utilities"; "categoryindex: 1"; "index: 3"], Some { StartLine = 2 StartColumn = 0 EndLine = 5 EndColumn = 8 }) to pynb markdown

*)
#r "nuget: FSharp.Data,4.2.4"
(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/diffsharp/diffsharp.github.io/master?filepath=library/HtmlParser.ipynb)&emsp;
[![Script](../img/badge-script.svg)](https://fsprojects.github.io/FSharp.Data//library/HtmlParser.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](https://fsprojects.github.io/FSharp.Data//library/HtmlParser.ipynb)

# HTML Parser

This article demonstrates how to use the HTML Parser to parse HTML files.

The HTML parser takes any fragment of HTML, uri or a stream and trys to parse it into a DOM.
The parser is based on the [HTML Living Standard](http://www.whatwg.org/specs/web-apps/current-work/multipage/index.html#contents)
Once a document/fragment has been parsed, a set of extension methods over the HTML DOM elements allow you to extract information from a web page
independently of the actual HTML Type provider.
*)
open FSharp.Data
(**
The following example uses Google to search for `FSharp.Data` then parses the first set of
search results from the page, extracting the URL and Title of the link.
We use the `cref:T:FSharp.Data.HtmlDocument` type.

To achieve this we must first parse the webpage into our DOM. We can do this using
the `cref:M:FSharp.Data.HtmlDocument.Load` method. This method will take a URL and make a synchronous web call
to extract the data from the page. Note: an asynchronous variant `cref:M:FSharp.Data.HtmlDocument.AsyncLoad` is also available
*)
let results = HtmlDocument.Load("http://www.google.co.uk/search?q=FSharp.Data")
(**
Now that we have a loaded HTML document we can begin to extract data from it.
Firstly we want to extract all of the anchor tags `a` out of the document, then
inspect the links to see if it has a `href` attribute, using `cref:M:FSharp.Data.HtmlDocumentExtensions.Descendants`. If it does, extract the value,
which in this case is the url that the search result is pointing to, and additionally the
`InnerText` of the anchor tag to provide the name of the web page for the search result
we are looking at.
*)
let links =
    results.Descendants ["a"]
    |> Seq.choose (fun x ->
           x.TryGetAttribute("href")
           |> Option.map (fun a -> x.InnerText(), a.Value())
    )
    |> Seq.toList
(**
Now that we have extracted our search results you will notice that there are lots of
other links to various Google services and cached/similar results. Ideally we would
like to filter these results as we are probably not interested in them.
At this point we simply have a sequence of Tuples, so F# makes this trivial using `Seq.filter`
and `Seq.map`.
*)
let searchResults =
    links
    |> List.filter (fun (name, url) ->
                    name <> "Cached" && name <> "Similar" && url.StartsWith("/url?"))
    |> List.map (fun (name, url) -> name, url.Substring(0, url.IndexOf("&sa=")).Replace("/url?q=", ""))
(**
Putting this all together yields the following:
```
[("FSharp.Data: Data Access Made Simplefsprojects.github.io › FSharp",
  "https://fsprojects.github.io/FSharp.Data/");
 ("fsprojects/FSharp.Data: F# Data: Library for Data Access - GitHubgithub.com › fsprojects › FSharp",
  "https://github.com/fsprojects/FSharp.Data/");
 ("FSharp.Data 4.2.3 - NuGetwww.nuget.org › packages › FSharp",
  "https://www.nuget.org/packages/FSharp.Data");
 ("Guide - Data Access | The F# Software Foundationfsharp.org › guides › data-access",
  "https://fsharp.org/guides/data-access/");
 ("Type Providers - F# | Microsoft Docsdocs.microsoft.com › Docs › .NET › F# guide",
  "https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/");
 ("Extracting data from websites with F# - Mark's Blogmallibone.com › post › ral-colour-table-with-fsharp",
  "https://mallibone.com/post/ral-colour-table-with-fsharp");
 ("How do I access FSharp.Data.JsonExtensions? - Stack Overflowstackoverflow.com › questions › how-do-i-access-fsharp-data-jsonextensions",
  "https://stackoverflow.com/questions/42542084/how-do-i-access-fsharp-data-jsonextensions");
 ("FSharp.Data.dll not found - Stack Overflow",
  "https://stackoverflow.com/questions/30065984/fsharp-data-dll-not-found");
 ("How to read in csv with FSharp.Data - Stack Overflow",
  "https://stackoverflow.com/questions/63759765/how-to-read-in-csv-with-fsharp-data");
 ("Is the FSharp.Data XML type provider generative or not?",
  "https://stackoverflow.com/questions/65465265/is-the-fsharp-data-xml-type-provider-generative-or-not");
 ("Using connectionstring from app.config with FSharp.Data.SqlClient",
  "https://stackoverflow.com/questions/40364229/using-connectionstring-from-app-config-with-fsharp-data-sqlclient");
 ("FSharp.Data (@FSharpData) | Twittertwitter.com › fsharpdata",
  "https://twitter.com/fsharpdata");
 ("F# Data: New type provider library - Tomas Petricektomasp.net › blog › fsharp-data",
  "http://tomasp.net/blog/fsharp-data.aspx/");
 ("F# Data Type Providers in .Net Core - Luke Merrettlukemerrett.com › fsharp-data-type-providers",
  "https://lukemerrett.com/fsharp-data-type-providers/");
 ("Learn more",
  "http://support.google.com/websearch%3Fp%3Dws_settings_location%26hl%3Den");
 ("Sign in",
  "https://accounts.google.com/ServiceLogin%3Fcontinue%3Dhttp://www.google.co.uk/search%253Fq%253DFSharp.Data%26hl%3Den")]
```

*)

