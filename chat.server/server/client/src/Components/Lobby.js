import { Form, Button } from 'react-bootstrap';
import { useState } from 'react';

const Lobby = ({ joinRoom, userName }) => {
const [room, setRoom] = useState();

  return (
    <div className="col-4 align-self-center">
    <Form className='lobby'
    onSubmit={ e => {
      e.preventDefault();
      joinRoom(room);
    }}>

      <div className="d-grid gap-2">
      <Form.Group>
        
        <div>{ userName }</div>

        <Form.Control
        placeholder="Room Name..." 
        onChange={e => setRoom(e.target.value)} />

      </Form.Group>
      
      
      <Button
      variant='success' 
      type='submit' 
      disabled={!room}
      >Join</Button>
      </div>
    </Form>
    </div>
  )
}

export default Lobby;