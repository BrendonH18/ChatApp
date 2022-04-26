import { Form, Button } from 'react-bootstrap';
import { useEffect, useState } from 'react';

const Login = ({connection, setNEW_User}) => {
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loginType, setLoginType] = useState('Select')


  const returnLoginAttempt = (User) => {
    connection.send("ReturnLoginAttempt", User)
  }
  const handleSubmit = () => {
    const User = {
      Username: username,
      Password: password,
      loginType: loginType
    }
    console.log(User)
    returnLoginAttempt(User)
    // returnIsValid(User)
    setUsername('')
    setPassword('');
  }

  useEffect(() => {
    formSelect();
  }, [loginType])

  const formSelect = () => {
    switch (loginType) {
      case "Returning":
        return <div>
          <Form.Group>
            <Form.Control 
              placeholder="Username..." 
              onChange={e => setUsername(e.target.value)} />
            <Form.Control
              placeholder="Password..." 
              onChange={e => setPassword(e.target.value)} />
          </Form.Group>
      
          <Button
            variant='success' 
            type='submit' 
            disabled={!username || !password}
          >User Login</Button>
        </div>;
      case "Create":
        return <div>
          <Form.Group>
            <Form.Control 
              placeholder="Username..." 
              onChange={e => setUsername(e.target.value)} />
            <Form.Control
              placeholder="Password..." 
              onChange={e => setPassword(e.target.value)} />
          </Form.Group>
      
          <Button
            variant='success' 
            type='submit' 
            disabled={!username || !password}
          >Create New User</Button>
        </div>;
      case "Guest":
        return <div>
            <Form.Group>
              <Form.Control 
                placeholder="Username..." 
                onChange={e => setUsername(e.target.value)} 
                />
            </Form.Group>
        
            <Button
              variant='success' 
              type='submit' 
              disabled={!username}
            >Continue as Guest</Button>
          </div>;
      default:
        return <></>
    }
  }

  return(
    <div className="col-4 align-self-center">
    <Form className='form'
    id="loginForm"
    onSubmit={ e => {
      e.preventDefault();
      handleSubmit()
    }}>
      <div className="d-grid gap-2">
      <h2>Please Login:</h2>
      <div className="btn-group" role="group" aria-label="Basic example">
        <button type="button" className="btn btn-secondary" onClick={e => setLoginType('Returning') }>Returning User</button>
        <button type="button" className="btn btn-secondary" onClick={e => setLoginType('Create') }>New User</button>
        <button type="button" className="btn btn-secondary"onClick={e => setLoginType('Guest') }>Guest</button>
      </div>
      {formSelect()}
      </div>
    </Form>
    </div>
  )
}

export default Login