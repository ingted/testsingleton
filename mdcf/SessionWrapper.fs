module Sessionrapper
open Akkling.Cluster.Sharding
open Akka.Actor
open Akka.Cluster
open Akka.Cluster.Sharding
open System
open Akkling
//open Akka.FSharp
open AkkaSingleton.Common
open AkkaSingleton.Common.Util.AkkaExtensions

open Akka.FSharp.Linq

open System.Threading
open System
open Akka.Cluster.Tools
open Akka.Cluster.Tools.Singleton
open Akka.Cluster.Tools
open Akka.Cluster.Tools.PublishSubscribe

let launch () = 
    printfn "launched----------------------"
    let cts = new CancellationTokenSource()
    let rec asfun () =
        async {
            printfn "orz"
            do! Async.Sleep 1000
            return! asfun ()
        } 
    asfun () |> (fun a -> Async.Start(a, cts.Token))
    printfn "returned----------------------"
    { new IDisposable 
        with 
            member __.Dispose () = 
                printfn "disposed----------------------"
                cts.Cancel ()
    }


let clusterSingleton (system:ActorSystem) launch singletonRole singletonName = 
    system.ActorOf(
        ClusterSingletonManager.Props(
            Props.CreateFromActorFn <| SingletonActor.create launch,
            PoisonPill.Instance,
            ClusterSingletonManagerSettings.Create(system).WithRole(singletonRole)),
            singletonName)


let configWithPort (port:int) =
    let config = Configuration.parse ("""
        akka {
          actor {
            provider = cluster
          }
          remote {
            dot-netty.tcp {
              public-hostname = "localhost"
              hostname = "localhost"
              port = """ + port.ToString() + """
            }
          }
          cluster {
            roles = ["Worker", "singletonRole"]
            sharding {
                role = "Worker"
                journal-plugin-id = "akka.persistence.journal.inmem"
                snapshot-plugin-id = "akka.persistence.snapshot-store.inmem"
            }
            seed-nodes = [ "akka.tcp://cluster-system@localhost:5000" ]
          }
          extensions = ["Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider,Akka.Cluster.Tools"]
        }
        """)
    config
      //.WithFallback(Akka.Cluster.Tools.Singleton.ClusterSingletonManager.DefaultConfig())
      .WithFallback(ClusterSharding.DefaultConfig())


