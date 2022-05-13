import { useState } from "react";
import { useParams } from "react-router-dom";
import { Form, Button } from 'react-bootstrap';

const User_CheckReturning = ({ connection }) => {

    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')

    const sendLoginAttempt = () => {
      const user = {
        Username: username,
        Password: password,
        LoginType: "Returning"
      }
      connection.send("ReturnLoginAttempt", user)
    }


    return(
      <>
      <h2>Returning</h2>
       <Form.Group>
            <Form.Control
              placeholder="Username..."
              onChange={e => setUsername(e.target.value)} />
            <Form.Control
              placeholder="Password..."
              onChange={e => setPassword(e.target.value)}
              />
          </Form.Group>

          <Button
            variant='success'
            type='submit'
            onClick={sendLoginAttempt}
            disabled={!username || !password}
          >User Login</Button>
      </>
    )
  }

  export default User_CheckReturning;