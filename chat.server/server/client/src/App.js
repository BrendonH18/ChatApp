import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import Landing from './Components/LoginComponents/LoginDashboard';
import ChannelDashboard from './Components/ChannelSelectionComponents/ChannelDashboard';
import ActiveChannel from './Components/ChannelSelectionComponents/ActiveChannel';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate, useParams, Outlet} from 'react-router-dom'

function App() {

  const blankUser = {
    id: 0,
    username: "",
    password: "",
    loginType: "",
    isPasswordValid: false
  }
  const user1 = {
    id: 1,
    username: "Brendon",
    password: "",
    loginType: "Returning",
    isPasswordValid: true
  }
  const user2 = {
    id: 2,
    username: "Steve",
    password: "",
    loginType: "Returning",
    isPasswordValid: true
  }
  const message1 = {
    username: "Brendon",
    text: "Hello World",
    created_on: Date()
  }

  const blankChannel = {
    id: 0,
    name: ""
  }

  const blankUserConnection = {
    user: blankUser,
    channel: blankChannel
  }

  const newChannels = [
    { id: 1, name: "Sports"},
    { id: 2, name: "Fashion"}
  ]

  //NEW
  const [connection, setConnection] = useState(null)
  const [isLoading, setIsLoading] = useState(false)
  const [messages, setMessages] = useState([message1])
  // const [user, setUser] = useState(blankUser)
  // const [channel, setChannel] = useState(blankChannel)
  const [userConnection, setUserConnection] = useState(blankUserConnection)
  const [availableChannels, setAvailableChannels] = useState(newChannels)
  // const [toggleDisplay, setToggleDisplay] = useState("Lobby")
  const [connectedUsers, setConnectedUsers] = useState([user1, user2])

  


  // useEffect(() => {
  //   setUserConnection({
  //     User: user,
  //     Channel: channel
  //   })
  // }, [user, channel])

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
          setUserConnection(x => {x.user = param})})
        connection.on("ReturnedChannel", (param) => {
          console.log("Returned Channel:", param)
          setUserConnection(x => {x.channel = param})})
        connection.on("ReturnedAvailableChannels", (param) => {
          console.log("Returned Available Channels:",param)
          setAvailableChannels( param )})
        // connection.on("ReturnedToggleDisplay", (param) => {
        //   console.log("Returned Toggle:",param)
        //   setToggleDisplay( param )})
        connection.on("ReturnedConnectedUsers", (param) => {
          console.log("Returned Connected Users:", param)
          setConnectedUsers(param)})

        connection.send("ReturnAvailableChannels")
        
      })
      .catch(e => console.log(e))
    }
  },[connection])

  // useEffect(() => {
  //   chatBody()
  // },[])

  // const chatBody = () => {
  //   if (user.isPasswordValid != true) 
  //     return <Login 
  //       connection={connection}
  //       setNEW_User={setUser}
  //       />
  //   return toggleDisplay === "Lobby"
  //     ?
  //       <Lobby
  //       connection={connection}
  //       userConnection={userConnection}
  //       setChannel={setChannel}
  //       availableChannels={availableChannels}
  //       />
  //     :    
  //       <Chat
  //       connection={connection}
  //       connectedUsers={connectedUsers}
  //       messages={messages}
  //       />
  // }


  // In-Bound
  // ReturnedMessage
  // ReturnedUser
  // ReturnedChannel
  // ReturnedAvailableChannels
  // ReturnedToggleDisplay
  // ReturnedConnectedUsers
  // LOBBY - ReturnedPasswordUpdate
  
  // Out-Bound
  // ReturnAvailableChannels
  // CHAT - LeaveChannel
  // LOBBY - JoinChannel
  // LOBBY - LogOut
  // LOBBY - UpdatePassword
  // LOGIN - ReturnLoginAttempt
  // SENDMESSAGEFORM - SendMessage


  return (
    <>
      <Router>
        <nav>
          <Link to="/">Home</Link>
          <Link to="/Login">User Login</Link>
          <Link to="/Channel">Channel Select</Link>
        </nav>
        <Routes>
          <Route path="/" element={<h2>Welcome Page</h2>}/>
          <Route path="/Channel/" element={<ChannelDashboard availableChannels={availableChannels} userConnection={userConnection} setUserConnection={setUserConnection} />}>
            <Route path=":ActiveChannel" element={<ActiveChannel messages={messages} connectedUsers={connectedUsers} userConnection={userConnection}/>}/>
          </Route>
          <Route path="/Login" element={<Landing userConnection={userConnection} setUserConnection={setUserConnection} />}>
            <Route path="Returning" element={<h2>Returning</h2>}/>
            <Route path="Create" element={<h2>Create</h2>}/>
            <Route path="Guest" element={<h2>Guest</h2>}/>
          </Route>
        </Routes>
      </Router>
    </>


    // <div className='app container-fluid vh-100 d-flex flex-column '>
    //   <h2 className="d-flex justify-content-center">My Chat 
    //   {/* {
    //     activeChannel
    //       ? ` - ${activeChannel.name}`
    //       : ""} */}
    //       </h2>
    //   <hr className='line'/>
    //       {/* {activeChannel
    //           ? <></>
    //           : <h2 className="d-flex justify-content-center">{loginMessage}</h2>} */}
    //   {chatBody()}
    // </div>
  );
}

export default App;
