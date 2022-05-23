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
import { BrowserRouter as Router, Routes, Route, useNavigate, Link} from 'react-router-dom'
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
  const [connectedUsers, setConnectedUsers] = useState(null)
  const [user, setUser] = useState(blankUser)
  const [channel, setChannel] = useState(blankChannel)


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
        setUser(param)
      })

      connection.on("ReturnedChannel", (param) => {
        setUserConnection({
          ...userConnection,
          channel: param
        })
        setChannel(param)
      })

      connection.on("ReturnedAvailableChannels", (param) => {
        setAvailableChannels( param )
        setIsConnectionLoading(false)
      })

      connection.on("ReturnedConnectedUsers", (param) => {
        setConnectedUsers(param)
      })
      
      connection.send("ConnectionSetup")
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
      <Router>
        <div className='container-fluid bg-dark text-white'>
          <nav className='d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3 mb-4 border-bottom'>
            <ul className='nav col-12 col-md-auto mb-2 justify-content-center mb-md-0'>
              <Link to="/" className="nav-link px-2 text-white">Home/Login</Link>
              <Link to="/Channel/1" className="nav-link px-2 text-white">Channels</Link>              
            </ul>
            <div className="col-md-3 text-end">
              <button type="button" className="btn btn-primary me-2">Log Out</button>
          </div>
          </nav>
        </div>
        <Routes>
          <Route path="/" element={<Home user={user} connection={connection} isConnectionLoading={isConnectionLoading}/>}/>
          <Route path="/Channel/:ActiveChannelID" element={<ChannelDashboard user={user} channel={channel} availableChannels={availableChannels} messages={messages} connectedUsers={connectedUsers} connection={connection} isConnectionLoading={isConnectionLoading} />}/>
        </Routes>
      </Router>
    </>
  );
}

export default App;
