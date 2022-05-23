import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import ChannelDashboard from './Components/ChannelDashboard';
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

  const [availableChannels, setAvailableChannels] = useState(null)
  const [channel, setChannel] = useState(blankChannel)
  const [connectedUsers, setConnectedUsers] = useState(null)
  const [connection, setConnection] = useState(null)
  const [messages, setMessages] = useState([])
  const [user, setUser] = useState(blankUser)


  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('/chat')
    .withAutomaticReconnect()
    .build();
    setConnection(newConnection)
  },[])

  useEffect(() => {
    if(connection === null) return
    connection.start()
    .then(result => {
      connection.on("ReturnedMessage", (param) => { 
        if (param === "Reset") return setMessages([])
        setMessages(m => [...m, param]) 
      })
      connection.on("ReturnedUser", (param) => setUser(param))
      connection.on("ReturnedChannel", (param) => setChannel(param))
      connection.on("ReturnedAvailableChannels", (param) => setAvailableChannels( param ))
      connection.on("ReturnedConnectedUsers", (param) => setConnectedUsers(param))
      connection.send("ConnectionSetup")
    })
    .catch(e => console.log(e))
    },[connection])

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
          <Route path="/" element={<Home user={user} connection={connection} />}/>
          <Route path="/Channel/:ActiveChannelID" element={<ChannelDashboard user={user} channel={channel} availableChannels={availableChannels} messages={messages} connectedUsers={connectedUsers} connection={connection} />}/>
        </Routes>
      </Router>
    </>
  );
}

export default App;
