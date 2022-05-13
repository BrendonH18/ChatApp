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
    if(connection){
      console.log("Connection Started")
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
          setIsConnectionLoading(false)})

        connection.on("ReturnedConnectedUsers", (param) => {
          setConnectedUsers(param)
        })
        
        connection.send("ReturnAvailableChannels")
      })
      .catch(e => console.log(e))
    }
  },[connection])

  const formatHeader = () => {
    if(userConnection.user.id != 0 ) return `Welcome ${userConnection.user.username}`
    return "Welcome Guest Viewer"
  }

  useEffect(() =>{
    formatHeader()
  },[userConnection])

  return (
    <>
    <div className='app container-fluid vh-100 d-flex flex-column '>
      <Router>
        <nav>
          <Link to="/">Home</Link>
          <Link to="/Login">User Login</Link>
          <Link to="/Channel">Channel Select</Link>
        </nav>
        <h2>{formatHeader()}</h2>
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
  );
}

export default App;
