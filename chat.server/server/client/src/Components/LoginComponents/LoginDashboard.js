import { Form, Button } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';

const LoginDashboard = ({userConnection, setUserConnection}) => {
  
  // const [username, setUsername] = useState('');
  // const [password, setPassword] = useState('');
  // const [loginType, setLoginType] = useState('Select')

  // const handleSubmit = () => {
  //   const User = {
  //     Username: username,
  //     Password: password,
  //     loginType: loginType
  //   }
  //   console.log(User)
  //   returnLoginAttempt(User)
  //   // returnIsValid(User)
  //   setUsername('')
  //   setPassword('');
  // }

  // const returnLoginAttempt = (User) => {
  //   connection.send("ReturnLoginAttempt", User)
  // }

  // useEffect(() => {
  //   formSelect();
  // }, [loginType])

  // const formSelect = () => {
  //   switch (loginType) {
  //     case "Returning":
  //       return <div>
  //         <Form.Group>
  //           <Form.Control 
  //             placeholder="Username..." 
  //             onChange={e => setUsername(e.target.value)} />
  //           <Form.Control
  //             placeholder="Password..." 
  //             onChange={e => setPassword(e.target.value)} />
  //         </Form.Group>
      
  //         <Button
  //           variant='success' 
  //           type='submit' 
  //           disabled={!username || !password}
  //         >User Login</Button>
  //       </div>;
  //     case "Create":
  //       return <div>
  //         <Form.Group>
  //           <Form.Control 
  //             placeholder="Username..." 
  //             onChange={e => setUsername(e.target.value)} />
  //           <Form.Control
  //             placeholder="Password..." 
  //             onChange={e => setPassword(e.target.value)} />
  //         </Form.Group>
      
  //         <Button
  //           variant='success' 
  //           type='submit' 
  //           disabled={!username || !password}
  //         >Create New User</Button>
  //       </div>;
  //     case "Guest":
  //       return <div>
  //           <Form.Group>
  //             <Form.Control 
  //               placeholder="Username..." 
  //               onChange={e => setUsername(e.target.value)} 
  //               />
  //           </Form.Group>
        
  //           <Button
  //             variant='success' 
  //             type='submit' 
  //             disabled={!username}
  //           >Continue as Guest</Button>
  //         </div>;
  //     default:
  //       return <></>
  //   }
  // }

  const newUser = {
    id: 1,
    username: "Brendon",
    password: "",
    loginType: "Returning",
    isPasswordValid: true
  }

  const oldUser = {
    id: 0,
    username: "",
    password: "",
    loginType: "",
    isPasswordValid: false
  }

  const toggleUser = () => {
    const values = {...userConnection}
    if (!userConnection.user.isPasswordValid) {
      values.user = newUser
      console.log(values)
      setUserConnection(values)
      navigate("/Channel")
      return
    }
    values.user = oldUser
    console.log(values)
    setUserConnection(values)
  }

  const navigate = useNavigate()

  return(
    <>
    <h1>Login Dashboard</h1>
    <button onClick={() => navigate("Create")}>Create</button>
    <button onClick={() => navigate("Returning")}>Returning</button>
    <button onClick={() => navigate("Guest")}>Guest</button>
    <button onClick={() => toggleUser()}>Update User</button>
    <h2>User Logged In? - {userConnection.user.isPasswordValid.toString()}</h2>
    <Outlet/>
    </>

    // <div className="col-4 align-self-center">
    // <Form className='form'
    // id="loginForm"
    // onSubmit={ e => {
    //   e.preventDefault();
    //   handleSubmit()
    // }}>
    //   <div className="d-grid gap-2">
    //   <h2>Please Login:</h2>
    //   <div className="btn-group" role="group" aria-label="Basic example">
    //     <button type="button" className="btn btn-secondary" onClick={e => setLoginType('Returning') }>Returning User</button>
    //     <button type="button" className="btn btn-secondary" onClick={e => setLoginType('Create') }>New User</button>
    //     <button type="button" className="btn btn-secondary"onClick={e => setLoginType('Guest') }>Guest</button>
    //   </div>
    //   {formSelect()}
    //   </div>
    // </Form>
    // </div>
  )
}

export default LoginDashboard