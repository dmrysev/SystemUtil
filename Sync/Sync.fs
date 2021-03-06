module SystemUtil.Sync

open Util.IO.Path

let removeRedundantDirectoriesInDestination (source: DirectoryPath) (destination: DirectoryPath) =
    let sourceDirectories = 
        Util.IO.Directory.listDirectoriesRecursive source
        |> Seq.map (DirectoryPath.relativeTo source)
    Util.IO.Directory.listDirectoriesRecursive destination
    |> Seq.map (DirectoryPath.relativeTo destination)
    |> Seq.except sourceDirectories
    |> Seq.map (fun relativeFilePath -> destination/relativeFilePath)
    |> Seq.iter Util.IO.Directory.delete

let removeRedundantFilesInDestination (source: DirectoryPath) (destination: DirectoryPath) =
    let sourceFiles = 
        Util.IO.Directory.listFilesRecursive source
        |> Seq.map (FilePath.relativeTo source)
    Util.IO.Directory.listFilesRecursive destination
    |> Seq.map (FilePath.relativeTo destination)
    |> Seq.except sourceFiles
    |> Seq.map (fun relativeFilePath -> destination/relativeFilePath)
    |> Seq.iter Util.IO.File.delete

let copyMissingFilesToDestination (source: DirectoryPath) (destination: DirectoryPath) =
    let destinationFiles = 
        Util.IO.Directory.listFilesRecursive destination
        |> Seq.map (FilePath.relativeTo destination)
    Util.IO.Directory.listFilesRecursive source
    |> Seq.map (FilePath.relativeTo source)
    |> Seq.except destinationFiles
    |> Seq.iter (fun relativeFilePath -> 
        let sourceFilePath = source/relativeFilePath
        let destFilePath = destination/relativeFilePath
        Util.IO.Directory.create destFilePath.DirectoryPath
        Util.IO.File.copy sourceFilePath destFilePath.Value)
     
let run (sourceDirPath: DirectoryPath) (destinationDirPath: DirectoryPath) =
    Util.IO.Directory.create destinationDirPath
    removeRedundantDirectoriesInDestination sourceDirPath destinationDirPath
    removeRedundantFilesInDestination sourceDirPath destinationDirPath
    copyMissingFilesToDestination sourceDirPath destinationDirPath
