// Learn more about F# at https://fsharp.org
// See the 'F# Tutorial' project for more help.


open Sessionrapper
open Akka.Actor
open Akkling.Cluster.Sharding
open Akka.Cluster
open Akka.Cluster.Sharding
open Akkling
open System
open System.Threading
open Akka.Cluster.Tools.PublishSubscribe


[<EntryPoint>]
let main argv =
    
    let system1 = ActorSystem.Create("cluster-system", configWithPort 5000)
    let system2 = ActorSystem.Create("cluster-system", configWithPort 5001)

    let singleMe = clusterSingleton system2 launch "singletonRole" "singleMe" 

 
    let l = System.Console.ReadLine()
    0 // return an integer exit code
