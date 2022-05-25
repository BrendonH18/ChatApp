import { faExclamationCircle, faUnderline } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import 'bootstrap/dist/css/bootstrap.min.css';
import {  useState } from 'react';



const Home = ({ user, setUser, connection }) => {

  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [isPasswordUpdated, setIsPasswordUpdated] = useState(null)
  

  const handleLogin = (e, type) => {
    e.preventDefault()
    if(!connection) return console.log("Not Connected")
    const user = {
      username: username,
      password: password,
      loginType: type
    }
    setUsername('')
    setPassword('')
    connection.send("ReturnLoginAttempt", user)
  }
  
  const handleUpdate = (e) => {
    e.preventDefault()
    const user = {
      password: password,
      newPassword: newPassword,
    }
    setPassword('')
    setNewPassword('')
    connection.on("ReturnedUpdatePassword", (param) => setIsPasswordUpdated(param))
    connection.send("UpdatePassword", user)
  }

  const formatPasswordUpdateButton = () => {
    if (isPasswordUpdated === true) return <button className="w-47 btn btn-lg btn-success" type="button" onClick={e => handleUpdate(e)}>Updated!</button>
    if (isPasswordUpdated === false) return <button className="w-47 btn btn-lg btn-danger" type="button" onClick={e => handleUpdate(e)}>Incorrect Password</button>
    return <button className="w-47 btn btn-lg btn-primary" type="button" onClick={e => handleUpdate(e)}>Update Password</button>
  }
  
  return(
    <>
    <div className='Home-Container col-xl-10 col-xxl-8 px-4 py-5'>
      <div className='row align-items-center g-lg-5 py-5'>
        <div className='column col-lg-7 text-center text-lg-start'>
          <h1 className="display-4 fw-bold lh-1 mb-3">Talk to Me: </h1>
          <p className="col-lg-10 fs-4">Explore the world through chat. Login to join the conversation, or observe as a guest!</p>
        </div>
        <div className="col-md-10 mx-auto col-lg-5">
          <form className='p-4 p-md-5 border rounded-3 bg-light'>
            {user && user.isPasswordValid
              ? <>
            <div className='form-floating mb-3'>
              <input type='password' className='form-control' id='floatingInput' placeholder='Old Password' value={password} onChange={e => setPassword(e.target.value)}/>
              <label htmlFor="floatingInput">Old Password</label>
            </div>
            <div className="form-floating mb-3">
              <input type='password' className="form-control" id="floatingPassword" placeholder="New Password" value={newPassword} onChange={e => setNewPassword(e.target.value)}/>
              <label htmlFor="floatingPassword">New Password</label>
            </div>
            </>
              : <>
            <div className='form-floating mb-3'>
              <input type='text' className='form-control' id='floatingInput' placeholder="Username" value={username} onChange={e => setUsername(e.target.value)}/>
              {user && !user.isPasswordValid && (user.loginType === "Returning" || user.loginType === "Guest")
                ?<>
                <label htmlFor="floatingInput" className='text-danger'><FontAwesomeIcon icon={faExclamationCircle}/> Username</label>    
                </>
                :<>
                <label htmlFor="floatingInput">Username</label>
                </>}
              
              {/* <label htmlFor="floatingInput" className='text-danger'><FontAwesomeIcon icon={faExclamationCircle}/> Username</label> */}
            </div>
            <div className="form-floating mb-3">
              <input type='password' className="form-control" id="floatingPassword" placeholder='Password' value={password} onChange={e => setPassword(e.target.value)}/>
              {user && !user.isPasswordValid && user.loginType === "Returning"
                ?<>
                <label htmlFor="floatingPassword" className='text-danger'><FontAwesomeIcon icon={faExclamationCircle}/> Password</label>    
                </>
                :<>
                <label htmlFor="floatingPassword">Password</label>
                </>}
              
              {/* <label htmlFor="floatingPassword">Password</label> */}
            </div>
            </>}
            <div className='row'>
              {user && user.isPasswordValid
              ? formatPasswordUpdateButton()
              : <div className='column text-center d-flex justify-content-around'>
                <button className="w-47 btn btn-lg btn-primary" type="button" onClick={e => handleLogin(e, "Create")}>Register</button>
                <button className="w-47 btn btn-lg btn-outline-secondary" type="button" onClick={e => handleLogin(e, "Returning")}>Sign in</button>
              </div>}
            </div>
            {user && user.isPasswordValid
            ? <></>
            : <><hr className="my-4"/>
            <div className='row'>
              <button className="w-47 btn btn-lg btn-secondary" type="button" onClick={e => handleLogin(e, "Guest")}>Continue as Guest</button>
            </div></>}
          </form>
        </div>
      </div>
    </div>
    </>
  )
}

export default Home