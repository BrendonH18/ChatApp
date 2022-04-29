import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import Lobby from './Components/Lobby';
import Chat from './Components/Chat';
import Login from './Components/Login';

function App() {
  //NEW
  const [connection, setConnection] = useState(null)
  const [messages, setMessages] = useState([])
  const [user, setUser] = useState({})
  const [channel, setChannel] = useState({})
  const [userConnection, setUserConnection] = useState({})
  const [availableChannels, setAvailableChannels] = useState([])
  const [toggleDisplay, setToggleDisplay] = useState("Lobby")
  const [connectedUsers, setConnectedUsers] = useState([])

  useEffect(() => {
    setUserConnection({
      User: user,
      Channel: channel
    })
  }, [user, channel])

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('/chat')
    .withAutomaticReconnect()
    //.configureLogging(LogLevel.Information)
    .build();
    setConnection(newConnection)
  },[])

  useEffect(() => {
    if(connection){
      connection.start()
      .then(result => {
        connection.on("ReturnedMessage", (param) => { 
          console.log("Returned Message:", param)
          if (param === "Reset")
            return setMessages([])
          setMessages(NEW_Messages => [...NEW_Messages, {param}]) 
        })
        connection.on("ReturnedUser", (param) => {
          console.log("Returned User:",param)
          setUser(param)})
        connection.on("ReturnedChannel", (param) => {
          console.log("Returned Channel:", param)
          setChannel(param)})
        connection.on("ReturnedAvailableChannels", (param) => {
          console.log("Returned Available Channels:",param)
          setAvailableChannels( param )})
        connection.on("ReturnedToggleDisplay", (param) => {
          console.log("Returned Toggle:",param)
          setToggleDisplay( param )})
        connection.on("ReturnedConnectedUsers", (param) => {
          console.log("Returned Connected Users:", param)
          setConnectedUsers(param)})

        connection.send("ReturnAvailableChannels")
        
      })
      .catch(e => console.log(e))
    }
  },[connection])

  useEffect(() => {
    chatBody()
  },[])

  const chatBody = () => {
    if (user.isPasswordValid != true) 
      return <Login 
        connection={connection}
        setNEW_User={setUser}
        />
    return toggleDisplay === "Lobby"
      ?
        <Lobby
        connection={connection}
        userConnection={userConnection}
        setChannel={setChannel}
        availableChannels={availableChannels}
        />
      :    
        <Chat
        connection={connection}
        connectedUsers={connectedUsers}
        messages={messages}
        />
  }

  return (
    <div className='app container-fluid vh-100 d-flex flex-column '>
      <h2 className="d-flex justify-content-center">My Chat 
      {/* {
        activeChannel
          ? ` - ${activeChannel.name}`
          : ""} */}
          </h2>
      <hr className='line'/>
          {/* {activeChannel
              ? <></>
              : <h2 className="d-flex justify-content-center">{loginMessage}</h2>} */}
      {chatBody()}
    </div>
  );
}

export default App;
