import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import Landing from './Components/LoginComponents/LoginDashboard';
import ChannelDashboard from './Components/ChannelSelectionComponents/ChannelDashboard';
import ActiveChannel from './Components/ChannelSelectionComponents/ActiveChannel';
import User_CheckReturning from './Components/LoginComponents/User_CheckReturning'
import User_CreateGuest from './Components/LoginComponents/User_CreateGuest';
import User_CreateNew from './Components/LoginComponents/User_CreateNew';
import User_UpdatePassword from './Components/LoginComponents/User_UpdatePassword';
import { BrowserRouter as Router, Routes, Route, Link} from 'react-router-dom'
import Home from './Components/Home';

function App() {

  const blankUser = {
    id: 0,
    username: "",
    password: "",
    loginType: "",
    isPasswordValid: false
  }

  const blankChannel = {
    id: 0,
    name: ""
  }

  const blankUserConnection = {
    user: blankUser,
    channel: blankChannel
  }

  const [connection, setConnection] = useState(null)
  const [isConnectionLoading, setIsConnectionLoading] = useState(true)
  const [messages, setMessages] = useState([])
  const [userConnection, setUserConnection] = useState(blankUserConnection)
  const [availableChannels, setAvailableChannels] = useState([])
  const [connectedUsers, setConnectedUsers] = useState([])

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('https://localhost:44314/chat')
    .withAutomaticReconnect()
    .build();
    setConnection(newConnection)
  },[])

  useEffect(() => {
    if(connection === null) return console.error("Connection Error")
    connection.start()
    .then(result => {
      connection.on("ReturnedMessage", (param) => { 
        if (param === "Reset")
          return setMessages([])
        setMessages(x => [...x, param]) 
      })

      connection.on("ReturnedUser", (param) => {
        setUserConnection({
          ...userConnection,
          user: param
        })
      })

      connection.on("ReturnedChannel", (param) => {
        setUserConnection({
          ...userConnection,
          channel: param
        })})

      connection.on("ReturnedAvailableChannels", (param) => {
        setAvailableChannels( param )
        setIsConnectionLoading(false)
      })

      connection.on("ReturnedConnectedUsers", (param) => {
        setConnectedUsers(param)
      })
      
      connection.send("ReturnAvailableChannels")
    })
    .catch(e => console.log(e))
    },[connection])

  const formatHeader = () => {
    if(userConnection.user.id != 0 ) return `Welcome ${userConnection.user.username}`
    return "Welcome Guest Viewer"
  }

  useEffect(() =>{
    console.log("UserConnection: ", userConnection)
  },[userConnection])

  useEffect(() =>{
    console.log("Users: ", connectedUsers)
  },[connectedUsers])
  return (
    <>
    <div className='container-fluid bg-dark text-white'>
      <nav className='d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3 mb-4 border-bottom'>
        <ul className='nav col-12 col-md-auto mb-2 justify-content-center mb-md-0'>
          <li><a href="/" className="nav-link px-2 text-white">Home/Login</a></li>
          <li><a className="nav-link px-2 text-white" href="/0">Channels</a></li>
        </ul>
        <div className="col-md-3 text-end">
          {/* <button type="button" className="btn btn-outline-primary me-2">Login</button> */}
      </div>
      </nav>
    </div>
    {/* <div className='app container-fluid vh-100 d-flex flex-column '> */}
      <Router>
        <Routes>
          <Route path="/" element={<Home userConnection={userConnection} isConnectionLoading={isConnectionLoading} connection={connection}/>}/>
          <Route path="/:ActiveChannelID" element={<ChannelDashboard connectedUsers={connectedUsers} messages={messages} isConnectionLoading={isConnectionLoading} setConnectedUsers={setConnectedUsers} setMessages={setMessages} connection={connection} availableChannels={availableChannels} userConnection={userConnection} setUserConnection={setUserConnection} />}/>
            {/* <Route path=":ActiveChannelID" element={<ActiveChannel isConnectionLoading={isConnectionLoading} availableChannels={availableChannels} connection={connection} messages={messages} connectedUsers={connectedUsers} userConnection={userConnection}/>}/> */}
          
          {/* <Route path="/Login" element={<Landing userConnection={userConnection} setUserConnection={setUserConnection} />}>
            <Route path="Returning" element={<User_CheckReturning connection={connection}/>}/>
            <Route path="Create" element={<User_CreateNew connection={connection}/>}/>
            <Route path="Guest" element={<User_CreateGuest connection={connection}/>}/>
            <Route path="Update" element={<User_UpdatePassword connection={connection}/>}/>
          </Route> */}
        </Routes>
      </Router>
    {/* </div> */}
    </>
  );
}

export default App;
