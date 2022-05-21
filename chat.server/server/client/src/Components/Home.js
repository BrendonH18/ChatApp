import 'bootstrap/dist/css/bootstrap.min.css';
import { Form, Button } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';



const Home = ({ user, channel, connection, isConnectionLoading, userConnection }) => {

  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')

  const handleClick = (e, type) => {
    e.preventDefault()
    console.log(isConnectionLoading)
    if(isConnectionLoading) return console.log("Not Connected")
    const user = {
      username: username,
      password: password,
      newPassword: newPassword,
      loginType: type
    }
    console.log(isConnectionLoading)
    console.log(user)
    console.log(connection)
    connection.send("ReturnLoginAttempt", user)
  }
  
  const isVisible = () => {
    if(user && user.isPasswordValid) return <>
    <hr className="my-4"/>
      <div className='col-md-10 mx-auto col-lg-5'>
        <div className=''>
          <div className="form-floating mb-3">
            <input type='password' className="form-control" id="floatingPassword" placeholder="NewPassword" onChange={e => setNewPassword(e.target.value)}/>
            <label for="floatingPassword">New Password</label>
          </div>
        </div>
      </div>
    <button className="w-47 btn btn-lg btn-secondary" type="button" onClick={e => handleClick(e, "Update")}>Update Password</button>
    <hr className="my-4"/>
    <button className="w-47 btn btn-lg btn-secondary" type="button" onClick={e => handleClick(e, "Logout")}>Logout</button>
    </>
    return <></>
  }

  return(
    <>
    <div className='Home-Container col-xl-10 col-xxl-8 px-4 py-5'>
      <div className='row align-items-center g-lg-5 py-5'>
        <div className='column col-lg-7 text-center text-lg-start'>
          <h1 className="display-4 fw-bold lh-1 mb-3">Talk to Me: </h1>
          <p className="col-lg-10 fs-4">Below is an example form built entirely with Bootstrap’s form controls. Each required form group has a validation state that can be triggered by attempting to submit the form without completing it.</p>
        </div>
        <div className="col-md-10 mx-auto col-lg-5">
          <form className='p-4 p-md-5 border rounded-3 bg-light'>
            <div className='form-floating mb-3'>
              <input type='text' className='form-control' id='floatingInput' placeholder='Username' onChange={e => setUsername(e.target.value)}/>
              <label for="floatingInput">Username</label>
            </div>
            <div className="form-floating mb-3">
              <input type='password' className="form-control" id="floatingPassword" placeholder="Password" onChange={e => setPassword(e.target.value)}/>
              <label for="floatingPassword">Password</label>
            </div>
            <div className='row'>
              <div className='column text-center d-flex justify-content-around'>
                <button className="w-47 btn btn-lg btn-primary" type="button" onClick={e => handleClick(e, "Create")}>Register</button>
                <button className="w-47 btn btn-lg btn-outline-secondary" type="button" onClick={e => handleClick(e, "Returning")}>Sign in</button>
              </div>
            </div>
            <hr className="my-4"/>
            <div className='row'>
              <button className="w-47 btn btn-lg btn-secondary" type="button" onClick={e => handleClick(e, "Guest")}>Continue as Guest</button>
            </div>
          </form>
        </div>
      </div>
    </div>
    </>
  )
}

export default Home