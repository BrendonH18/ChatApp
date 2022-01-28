import { Form, Button } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import reactDom from 'react-dom';

const Login = ({userValidation, signalRConnection}) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loginType, setLoginType] = useState('Select')
  const [isGuest, setIsGuest] = useState(false)
  
  const FormUpdate = () => {
    switch (loginType) {
      case "Returning":
        return <div>
          
          <Button
          variant='success' 
          type='submit' 
          disabled={!username || !password}
          >User Login</Button>
          </div>
      case "Create":
        return <div>
          {/* <Form.Group>
            <Form.Control 
          placeholder="Username..." 
          onChange={e => setUsername(e.target.value)} />
            <Form.Control
          placeholder="Password..." 
          onChange={e => setPassword(e.target.value)} />
          </Form.Group> */}
          <Button
          variant='success' 
          type='submit' 
          disabled={!username || !password}
          >Create User</Button>
          </div>
      case "Guest":
        return <div>
          {/* <Form.Group>
            <Form.Control 
          placeholder="Username..." 
          onChange={e => setUsername(e.target.value) } />
          </Form.Group> */}
          <Button
          variant='success' 
          type='submit' 
          disabled={!username}
          >Continue as Guest</Button></div>
      default:
        return <></>
    }
  }

  useEffect(() => {
    FormUpdate();
  }, [loginType])

  return(
    <div className="col-4 align-self-center">
    <Form className='form'
    id="loginForm"
    // ref= {form => this.loginForm = form}
    onSubmit={ e => {
      e.preventDefault();
      const param = {
        Username: username,
        Password: password,
        loginType: loginType
      }
      // signalRConnection("userValidation", params)
      userValidation(param)
      setUsername('')
      setPassword('');
    }}>
      
      <div className="d-grid gap-2">
      <div class="btn-group" role="group" aria-label="Basic example">
        <button type="button" class="btn btn-secondary" onClick={e => setLoginType('Returning') }>Returning User</button>
        <button type="button" class="btn btn-secondary" onClick={e => setLoginType('Create') }>New User</button>
        <button type="button" class="btn btn-secondary"onClick={e => setLoginType('Guest') }>Guest</button>
      </div>

      <Form.Group>
            <Form.Control 
            placeholder="Username..." 
            onChange={e => setUsername(e.target.value)} />
            <Form.Control
            placeholder="Password..." 
            onChange={e => setPassword(e.target.value)} />
          </Form.Group>
      <FormUpdate/>
      
      </div>
    </Form>
    </div>
  )
}

export default Login