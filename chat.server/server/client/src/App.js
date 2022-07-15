import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import ChannelDashboard from './Components/ChannelDashboard';
import { BrowserRouter as Router, Routes, Route, Link} from 'react-router-dom'
import Home from './Components/Home';
import { Offcanvas } from 'bootstrap';
import { useLocalStorage } from './Components/useLocalStorage';

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
  const [channel, setChannel] = useLocalStorage('channel', blankChannel)
  const [connectedUsers, setConnectedUsers] = useState(null)
  const [connection, setConnection] = useState(null)
  const [messages, setMessages] = useState([])
  const [user, setUser] = useLocalStorage('user', blankUser)
  const [resetPassword, setResetPassword] = useState('')
  const [resetNewPassword, setResetNewPassword] = useState('')
  const [isPasswordUpdated, setIsPasswordUpdated] = useState()
  const [isInitialLogin, setIsInitialLogin] = useState(true)
  const [token, setToken] = useLocalStorage('token', 0)
  const [count, setCount] = useLocalStorage('count', 0)
  
  useEffect(()=>{
    const newcount = count + 1
    setCount(newcount)
    console.log("Count", count)
  },[token])

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('/chat', { accessTokenFactory: () => token})
    .withAutomaticReconnect()
    .build();
    setConnection(newConnection)
  },[token])

  useEffect(() => {
    if(connection === null) return
    connection.start()
    .then(() => {
      const param = {
        username: "jason_admin",
      }
      connection.on("ReturnedStartUpValidation", (param)=>console.log("Start Up:", param))
      connection.send("ReturnStartUpValidation", param)
      debugger
    })
    .then(result => {
      connection.on("ReturnedMessage", (param) => { 
        if (param === "Reset") return setMessages([])
        setMessages(m => [...m, param]) 
      })
      connection.on("ReturnedUser", (param) => setUser(param))
      connection.on("ReturnedChannel", (param) => setChannel(param))
      connection.on("ReturnedAvailableChannels", (param) => setAvailableChannels( param ))
      connection.on("ReturnedConnectedUsers", (param) => setConnectedUsers(param))
      connection.on("ReturnedUpdatePassword", (param) => {
        setIsPasswordUpdated(param)
        console.log("Password update: ",param)
      })
      connection.on("ReturnedJWTTest", (param) => console.log("ReturnedJWTTest", param))
      connection.send("ConnectionSetup")
    })
    .catch(e => console.log(e))
    },[connection])

    const handleLogout = (e) => {
      e.preventDefault()
      if(!connection) return
      connection.send("Logout")
      setIsInitialLogin(true)
    }

    const handlePasswordReset = (e) => {
      e.preventDefault()
      const user = {
        password: resetPassword,
        newPassword: resetNewPassword,
      }
      setResetPassword('')
      setResetNewPassword('')
      connection.send("UpdatePassword", user)
    }

  return (
    <>
      <Router>
        <div className='container-fluid bg-dark text-white'>
          <nav className='d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3 mb-4 border-bottom'>
            <ul className='nav col-12 col-md-auto mb-2 justify-content-center mb-md-0'>
              <Link to="/" className="nav-link px-2 text-white">Home/Login</Link>
              <Link to={`/Channel/${availableChannels?availableChannels[0]["id"]:1}`} className="nav-link px-2 text-white">Channels</Link>              
            </ul>
          {user && user.isPasswordValid
            ? <>
            <div className='col-md-3 text-center'>
                <div className='px-2 text-white'>{`Welcome, ${user.username}!`}</div>
            </div>
            <div className="col-md-3text-end">
              <button class="btn btn-primary" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasRight" aria-controls="offcanvasRight">Update Password</button>
              <button type="button" className="btn btn-primary me-2" onClick={e => handleLogout(e)}>Log Out</button>
            </div>
            </>
            : <></>}
          </nav>
        </div>
<div className="offcanvas offcanvas-end" tabindex="-1" id="offcanvasRight" aria-labelledby="offcanvasRightLabel">
  <div className="offcanvas-header">
    <h5 id="offcanvasRightLabel">Update Password</h5>
    <button type="button" className="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close" ></button>
  </div>
  <div className="offcanvas-body">
    <div className='form-floating mb-3'>
      <input type='password' className='form-control' id='floatingInput' placeholder='Old Password' value={resetPassword} onChange={e => setResetPassword(e.target.value)}/>
      <label htmlFor="floatingInput">Old Password</label>
    </div>
    <div className="form-floating mb-3">
      <input type='password' className="form-control" id="floatingPassword" placeholder="New Password" value={resetNewPassword} onChange={e => setResetNewPassword(e.target.value)}/>
      <label htmlFor="floatingPassword">New Password</label>
    </div>
    <button className="btn btn-lg btn-primary" type="button" onClick={e => handlePasswordReset(e)}>Update Password</button>
  </div>
  {isPasswordUpdated && isPasswordUpdated.isPasswordApproved
    ? <div>Password Updated!</div>
    : <div>Password FAILED!</div>}
  <div className='offcanvas-footer'>
  </div>
</div>
        <Routes>
          <Route path="/" element={<Home setToken={setToken} user={user} channel={channel} setUser={setUser} connection={connection} isInitialLogin={isInitialLogin} setIsInitialLogin={setIsInitialLogin} firstChannelId={availableChannels?availableChannels[0]["id"]:1} setMessages={setMessages} setChannel={setChannel} blankChannel={blankChannel}/>}/>
          <Route path="/Channel/:ActiveChannelID" element={<ChannelDashboard user={user} channel={channel} availableChannels={availableChannels} messages={messages} connectedUsers={connectedUsers} connection={connection} />}/>
        </Routes>
      </Router>
    </>
  );
}

export default App;
