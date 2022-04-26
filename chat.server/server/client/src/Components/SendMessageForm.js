import { Form, Button } from "react-bootstrap";
import { useState } from 'react';

const SendMessageForm = ({ connection }) => {
  
  const [text, setText] = useState('');

  const sendMessage = (param) =>{
      connection.send("SendMessage", param);
  }

  return(
    
    <Form className='send-message-form' // fixed-bottom
    onSubmit={ e => {
      e.preventDefault();
      sendMessage(text);
      setText('');
    }}>
      <div className="row">
      <div className="col-9">
      <Form.Group>
        
        <Form.Control 
        placeholder="Your message..." 
        onChange={e => setText(e.target.value)}
        />
        
      </Form.Group>
      </div>

      <div className="col-3 d-grid">
      <Button 
      variant='success' 
      type='submit' 
      >Send</Button>
      </div>
      </div>
    
    </Form>
    
  )
}

export default SendMessageForm;