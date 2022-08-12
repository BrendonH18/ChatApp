import { faExclamationCircle, faUnderline } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import 'bootstrap/dist/css/bootstrap.min.css';
import {  useEffect, useState } from 'react';
import {useNavigate} from 'react-router-dom'
import axios from 'axios';


const Home = ({ setJwt, jwt, user, isInitialLogin, setIsInitialLogin, connection, firstChannelId, setChannel, setMessages, blankChannel }) => {

  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')

  let navigate = useNavigate()

  const handleLogin = (e, type) => {
    e.preventDefault()
    if(!connection) return console.log("Not Connected")
    const user1 = {
      Username: username,
      Password: password,
      loginType: type
    }
    setUsername('')
    setPassword('')
    const authAxios = axios.create({
      baseURL: "https://localhost:44314/",
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    authAxios.post("api/login/", user1)
    .then( res => {
      if(res.status == 200) setJwt(res.data)
    }
    )
    // connection.send("ReturnLoginAttempt", user1)
  }

  useEffect(() => {
    if(!user.isPasswordValid) return
    if(!isInitialLogin) return
    setIsInitialLogin(false)
    navigate(`/Channel/${firstChannelId}`)
  }, [user])

  useEffect(() => {
    setMessages([])
    setChannel(blankChannel)
  }, [])
  
  // const formatPasswordUpdateButton = () => {
  //   if (isPasswordUpdated === true) return <button className="w-47 btn btn-lg btn-success" type="button" onClick={e => handleUpdate(e)}>Updated!</button>
  //   if (isPasswordUpdated === false) return <button className="w-47 btn btn-lg btn-danger" type="button" onClick={e => handleUpdate(e)}>Incorrect Password</button>
  //   return <button className="w-47 btn btn-lg btn-primary" type="button" onClick={e => handleUpdate(e)}>Update Password</button>
  // }
  
  return(
    <>
    <div className='Home-Container col-xl-10 col-xxl-8 px-4 py-5'>
      <div className='row align-items-center g-lg-5 py-5'>
        <div className='column col-lg-7 text-center text-lg-start'>
          <h1 className="display-4 fw-bold lh-1 mb-3">Talk to Me: </h1>
          <p className="col-lg-10 fs-4">Explore the world through chat. Login to join the conversation, or observe as a guest!</p>
        </div>
            {user && user.isPasswordValid
              ? <></>
              : <>
        <div className="col-md-10 mx-auto col-lg-5">
          <form className='p-4 p-md-5 border rounded-3 bg-light'>
            <div className='form-floating mb-3'>
              <input type='text' className='form-control' id='usernameInput' placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} required/>
              <label htmlFor="usernameInput">Username</label>
              {user && !user.isPasswordValid && (user.loginType === "Returning")
                ?<>
              <div className='text-danger'>
                <FontAwesomeIcon icon={faExclamationCircle}/>  Please check your username
              </div>
                </>
                :<></>}
                {user && !user.isPasswordValid && user.loginType === "Guest"
                ?<>
              <div className='text-danger'>
                <FontAwesomeIcon icon={faExclamationCircle}/>  Please enter a username
              </div>
                </>
                :<></>}
            </div>
            <div className="form-floating mb-3">
              <input type='password' className="form-control" id="floatingPassword" placeholder='Password' value={password} onChange={e => setPassword(e.target.value)}/>
              <label htmlFor="floatingPassword">Password</label>
              {user && !user.isPasswordValid && user.loginType === "Returning"
                ?<div className='text-danger'>
                  <FontAwesomeIcon icon={faExclamationCircle}/> Please check your password
                </div>
                :<></>}
            </div>
            <div className='row'>
              {user && user.isPasswordValid
              ? <></>
              : <div className='column text-center d-flex justify-content-around'>
                <button className="w-47 btn btn-lg btn-primary" type="button" onClick={e => handleLogin(e, "Create")}>Register</button>
                <button className="w-47 btn btn-lg btn-outline-secondary" type="button" onClick={e => handleLogin(e, "Returning")}>Sign in</button>
              </div>}
            </div>
            {user && user.isPasswordValid
              ? <></>
              : <>
            <hr className="my-4"/>
            <div className='row'>
              <button className="w-47 btn btn-lg btn-secondary" type="button" onClick={e => handleLogin(e, "Guest")}>Continue as Guest</button>
            </div></>}
          </form>
        </div> 
              </>}
      </div>
    </div>
    </>
  )
}

export default Home