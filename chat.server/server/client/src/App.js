import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import Landing from './Components/LoginComponents/LoginDashboard';
import ChannelDashboard from './Components/ChannelSelectionComponents/ChannelDashboard';
import ActiveChannel from './Components/ChannelSelectionComponents/ActiveChannel';
import User_CheckReturning from './Components/LoginComponents/User_CheckReturning'
import User_CreateGuest from './Components/LoginComponents/User_CreateGuest';
import User_CreateNew from './Components/LoginComponents/User_CreateNew';
import User_UpdatePassword from './Components/LoginComponents/User_UpdatePassword';
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
    user: {username: "Brendon"},
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
  const [isConnectionLoading, setIsConnectionLoading] = useState(true)
  const [messages, setMessages] = useState([])
  // const [user, setUser] = useState(blankUser)
  // const [channel, setChannel] = useState(blankChannel)
  const [userConnection, setUserConnection] = useState(blankUserConnection)
  const [availableChannels, setAvailableChannels] = useState([])
  // const [toggleDisplay, setToggleDisplay] = useState("Lobby")
  const [connectedUsers, setConnectedUsers] = useState([])

  


  // useEffect(() => {
  //   setUserConnection({
  //     User: user,
  //     Channel: channel
  //   })
  // }, [user, channel])

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('https://localhost:44314/chat')
    .withAutomaticReconnect()
    //.configureLogging(LogLevel.Information)
    .build();
    setConnection(newConnection)
  },[])

  useEffect(() => {
    if(connection){
      console.log("Connection Started")
      connection.start()
      .then(result => {
        

        //Returned
        connection.on("ReturnedMessage", (param) => { 
          console.log("Returned Message:", param)
          if (param === "Reset")
            return setMessages([])
          setMessages(x => [...x, param]) 
        })

        connection.on("ReturnedUser", (param) => {
          console.log("Returned User:",param)
          const newUserConnection = userConnection
          newUserConnection.user = param
          setUserConnection(newUserConnection)
        })

        connection.on("ReturnedChannel", (param) => {
          console.log("Returned Channel:", param)
          const newUserConnection = userConnection
          newUserConnection.channel = param
          setUserConnection(newUserConnection)})

        connection.on("ReturnedAvailableChannels", (param) => {
          console.log("Returned Available Channels:",param)
          setAvailableChannels( param )
          setIsConnectionLoading(false)})

        // connection.on("ReturnedToggleDisplay", (param) => {
        //   console.log("Returned Toggle:",param)
        //   setToggleDisplay( param )})

        connection.on("ReturnedConnectedUsers", (param) => {
          console.log("Returned Connected Users:", param)
          setConnectedUsers(param)
        })
        
        //Send
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
    <div className='app container-fluid vh-100 d-flex flex-column '>
      <Router>
        <nav>
          <Link to="/">Home</Link>
          <Link to="/Login">User Login</Link>
          <Link to="/Channel">Channel Select</Link>
        </nav>
        <Routes>
          <Route path="/" element={<h2>Welcome Page</h2>}/>
          <Route path="/Channel/" element={<ChannelDashboard setConnectedUsers={setConnectedUsers} setMessages={setMessages} connection={connection} availableChannels={availableChannels} userConnection={userConnection} setUserConnection={setUserConnection} />}>
            <Route path=":ActiveChannelID" element={<ActiveChannel isConnectionLoading={isConnectionLoading} availableChannels={availableChannels} connection={connection} messages={messages} connectedUsers={connectedUsers} userConnection={userConnection}/>}/>
          </Route>
          <Route path="/Login" element={<Landing userConnection={userConnection} setUserConnection={setUserConnection} />}>
            <Route path="Returning" element={<User_CheckReturning connection={connection}/>}/>
            <Route path="Create" element={<User_CreateNew connection={connection}/>}/>
            <Route path="Guest" element={<User_CreateGuest connection={connection}/>}/>
            <Route path="Update" element={<User_UpdatePassword connection={connection}/>}/>
          </Route>
        </Routes>
      </Router>
    </div>
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
