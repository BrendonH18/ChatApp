import { Form, Button } from 'react-bootstrap';
import { useState } from 'react';

const Lobby = ({ joinRoom }) => {
const [user, setUser] = useState();
const [room, setRoom] = useState();

  return (
    <div className="col-4 align-self-center">
    <Form className='lobby'
    onSubmit={ e => {
      e.preventDefault();
      joinRoom(user, room);
    }}>

      <div className="d-grid gap-2">
      <Form.Group>
        
        <Form.Control 
        placeholder="Your Name..." 
        onChange={e => setUser(e.target.value)} />

        <Form.Control
        placeholder="Room Name..." 
        onChange={e => setRoom(e.target.value)} />

      </Form.Group>
      
      
      <Button
      variant='success' 
      type='submit' 
      disabled={!user || !room}
      >Join</Button>
      </div>
    </Form>
    </div>
  )
}

export default Lobby;