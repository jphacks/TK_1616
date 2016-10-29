(defproject text "0.1.0-SNAPSHOT"
  :description "FIXME: write description"
  :url "http://example.com/FIXME"
  :min-lein-version "2.0.0"
  :dependencies [[org.clojure/clojure "1.8.0"]
                 [compojure "1.5.1"]
                 [ring/ring-defaults "0.2.1"]
                 [ring/ring-core "1.4.0"]
                 [ring/ring-jetty-adapter "1.2.0"]
                 [environ "0.2.1"]
                 [http-kit "2.1.2"]
                 [ring "1.4.0"]]
  :plugins [[lein-ring "0.9.7"]]
  :ring {:handler text.handler/app}
  :main text.handler
  :profiles
  {:dev {:dependencies [[javax.servlet/servlet-api "2.5"]
                        [ring/ring-mock "0.3.0"]]}})
