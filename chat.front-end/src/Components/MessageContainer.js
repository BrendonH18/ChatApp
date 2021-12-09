
const MessageContainer = ({ messages }) => {

  return(
    // <div className="d-grid">
    <div className='message-container'>
      {messages.map((message, index) => {
        return(
        <div className='message row d-grid justify-content-end' key={index}>

          <div className="col-auto">
          <div className='user'>{message.user}:</div>
          

          <div className='text d-grid justify-content-end'>{message.text}</div>
          </div>

        </div>
      )})}
    </div>
    // </div>
  )
}

export default MessageContainer;