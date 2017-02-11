#r "./tools/FAKE/tools/FakeLib.dll"

open Fake
open System
open System.Diagnostics
open System.Text

type NVikaParseReportParams = {
    /// Active the debug category on logger, useful for debugging. Default `false`.
    /// Equivalent to the `--debug` option.
    Debug: bool
    /// Include the report source name in messages. Default `false`.
    /// Equivalent to the `--includesource` option.
    IncludeSource: bool
    /// The path to NVika. Default search in the "tools" folder.
    ToolPath : string
    /// Maximum time to allow NVika to run before being killed.
    TimeOut : TimeSpan
}

/// Containes tasks which allow to call nvika
module NVika =
    /// The default option set given to choco install.
    let NVikaParseReportDefaults = {
        Debug = false
        IncludeSource = false
        ToolPath = null
        TimeOut = TimeSpan.FromMinutes 5.
    }
    
    /// [omit]
    /// Tries to find the specified choco executable:
    ///
    /// 1. In the `<ProgramData>\chocolatey\bin` directory
    /// 2. In the `PATH` environment variable.
    let private findExe toolPath =
        if toolPath |> isNullOrEmpty then
            let paths = [
                            Seq.singleton (findToolInSubPath "NVika.exe" ("tools" @@ "nvika"))
                            Seq.singleton ("tools" @@ "NVika")
                            Seq.singleton (environVar "ProgramData" @@ "chocolatey" @@ "lib" @@ "nvika")
                            pathDirectories
                        ] |> Seq.concat
            let found = paths
                        |> Seq.map (fun directory -> if hasExt ".exe" directory then directory else directory @@ "NVika.exe")
                        |> Seq.tryFind fileExists
        
            if found <> None 
            then 
                trace (sprintf "founded: %s" found.Value)
                found.Value 
            else 
                failwith (sprintf "Cannot find the NVika executable in following paths: %A" paths)
        else toolPath

    /// [omit]
    let private buildArgs command reports parameters =
        new StringBuilder()
                |> appendWithoutQuotes command
                |> appendWithoutQuotes (separated " " reports)
                |> appendIfTrueWithoutQuotes parameters.Debug "--debug"
                |> appendIfTrueWithoutQuotes parameters.IncludeSource "--includesource"
                |> toText

    /// Runs NVika parse report on the given reports.
    ///
    /// ## Parameters
    ///
    ///  - `setParams` - Function used to manipulate the default `XUnit2Params` value.
    ///  - `reports` - Sequence of one or more analysis reports.
    ///
    /// ## Sample usage
    ///
    ///     Target "ParseReport" (fun _ ->
    ///         !! (buildDir @@ "MyApp.*.CodeAnalysisLog.xml")
    ///         |> NVika.ParseReports (fun p -> { p with IncludeSource = true })
    ///     )
    let ParseReports setParams reports =
        let details = separated ", " reports
        use __ = traceStartTaskUsing "NVika.ParseReports" details

        if reports |> isNull then failwith "'reports' must not be empty."

        let parameters = setParams NVikaParseReportDefaults

        let args = buildArgs "parsereport" reports parameters

        let result =
            ExecProcess (fun info ->
                info.FileName <- findExe parameters.ToolPath
                info.Arguments <- args) parameters.TimeOut
        if result <> 0 then failwithf "NVika.ParseReports failed with exit code %i." result
        
    /// Runs NVika parse report on the given reports.
    ///
    /// ## Parameters
    ///
    ///  - `setParams` - Function used to manipulate the default `XUnit2Params` value.
    ///  - `report` - Path of the analysis report.
    ///
    /// ## Sample usage
    ///
    ///     Target "ParseReport" (fun _ ->
    ///         "MyApp.exe.CodeAnalysisLog.xml"
    ///         |> NVika.ParseReport (fun p -> { p with IncludeSource = true })
    ///     )
    let ParseReport setParams report =
        use __ = traceStartTaskUsing "NVika.ParseReport" report

        if report |> isNullOrEmpty then failwith "'report' must not be empty."

        let parameters = setParams NVikaParseReportDefaults

        let args = buildArgs "parsereport" [report] parameters

        let result =
            ExecProcess (fun info ->
                info.FileName <- findExe parameters.ToolPath
                info.Arguments <- args) parameters.TimeOut
        if result <> 0 then failwithf "NVika.ParseReport failed with exit code %i." result
