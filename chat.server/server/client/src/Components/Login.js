import { Form, Button } from 'react-bootstrap';
import { useEffect, useState } from 'react';

const Login = ({connection}) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loginType, setLoginType] = useState('Select')

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

  const returnIsValid = (param) => {
    try {
      connection.send("ReturnIsValid", param)
    } catch (error) {
      console.log(error)
    }    
  }

  const handleSubmit = () => {
    const param = {
      Username: username,
      Password: password,
      loginType: loginType
    }
    returnIsValid(param)
    setUsername('')
    setPassword('');
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
      <div class="btn-group" role="group" aria-label="Basic example">
        <button type="button" class="btn btn-secondary" onClick={e => setLoginType('Returning') }>Returning User</button>
        <button type="button" class="btn btn-secondary" onClick={e => setLoginType('Create') }>New User</button>
        <button type="button" class="btn btn-secondary"onClick={e => setLoginType('Guest') }>Guest</button>
      </div>
      {formSelect()}
      </div>
    </Form>
    </div>
  )
}

export default Login