### kafka-dotnet-wpf-endpoint

<h4> General Info </h4>
<ol>
  <li> 
    The purpose of this application is enable a Kafka endpoint simulator for testing Kafka, Kafka Connect and Kafka Streams with simulated analytics data streams
  </li>
  <li>
    In this test application, confluent kafka C#.NET driver is used to spawn a producer and consumer which are each enabled in a separate threads 
    <ul>
      <li> Currently the producer and consumer defaults to bootstrap_servers='localhost:9092. Enter info in side nav menu for bootstrap-server and topics, etc.. </li>
      <li> There is an epd.conf file that should be edited to make changes in the simulator outputs. 
    </ul>
  </li>
  <li> 
    The UI page is basic WPF so there is considerable flexibility in its application. There are currently start and stop buttons.
  </li>
  <li> 
    WPF is used as this endpoint is intended for application on equipment and sensors where .NET implementations are very common
  </li>
</ol>
