import { useParams } from "react-router-dom";
import { Form, Button } from 'react-bootstrap';
import { useState } from "react";

const User_CreateNew = ({ connection }) => {
  
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loginType, setLoginType] = useState("Create")

  const sendLoginAttempt = () => {
    const user = {
      Username: username,
      Password: password,
      LoginType: loginType
    }
    console.log("ReturnLoginAttempt:", user)
    connection.send("ReturnLoginAttempt", user)
  }

    return(
      <>
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
            onClick={sendLoginAttempt} 
            disabled={!username || !password}
          >Create New User</Button>
      </>
    )
  }
  
  export default User_CreateNew;