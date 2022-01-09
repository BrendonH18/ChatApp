import { Form, Button } from 'react-bootstrap';
import { useState } from 'react';

const Login = ({userValidation}) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  
  return(
    <div className="col-4 align-self-center">
    <Form className='lobby'
    onSubmit={ e => {
      e.preventDefault();
      userValidation(username, password);
    }}>

      <div className="d-grid gap-2">
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
      >Submit</Button>
      </div>
    </Form>
    </div>
  )
}

export default Login