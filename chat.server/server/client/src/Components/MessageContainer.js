import { useEffect, useRef } from 'react';

const MessageContainer = ({ messages }) => {
  
  const messagesEndRef = useRef(null)
  const scrollToBottom = () => messagesEndRef.current?.scrollIntoView({behavior: "smooth"})
  useEffect(scrollToBottom, [messages])

  const formatDate = (param) => {
    var pattern = "HH:mm M/d/yyy" 
    var date = new Date(param)
    return date.toLocaleString()
  }
  
  return(
    <div className='message-container' style={{overflowX: "hidden", overflowY: "scroll"}}>
      {messages.map((message, index) => {
        return(
        <div className='message row d-grid' key={index}>
          <div className="">
          <div className='user'>{message.param.username} - {formatDate(message.param.created_on)}:</div>
          <div className='text'>{message.param.text}</div>
          </div>
        </div>
      )})}
      <div ref={messagesEndRef}/>
    </div>
  )
}

export default MessageContainer;