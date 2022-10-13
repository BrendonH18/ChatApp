import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faPaperPlane } from "@fortawesome/free-solid-svg-icons"
import * as methods from "../../Methods"
import { useEffect, useRef, useState } from "react"
import axios from "axios"

const Messages = ({channel, user, messages, jwt}) => {

    const messagesEndRef = useRef(null)
    const [messageText, setMessageText] = useState('')

    const scrollToBottom = () => messagesEndRef.current?.scrollIntoView({behavior: "smooth"})
    const handleMessageInput = (e) => {
        setMessageText(e.target.value)
    }

    const handleMessageSubmitWithEnterKey = (e) => {
        if (!user.isPasswordValid) return
        if (messageText === "") return
        if (e.code === "Enter") handleSendMessage(e)
    }
    const handleSendMessage = (e) =>{
        e.preventDefault()
        // if (!user.isPasswordValid) return setMessageText("Message blocked - Please login to send messages")
        const message = {
          text: messageText,
          isBot: false
        }
        setMessageText('')
        const authAxios = axios.create({
            baseURL: "https://localhost:44314/",
            headers: {
                Authorization: `Bearer ${jwt}`
            }
        })
        
        authAxios.post("api/chat/sendmessage", message)
        .then(res => console.log(res));
      }


    useEffect(scrollToBottom, [messages])

return <>
    <div className="card">
        <div className="card-header msg_head">
            <div className="d-flex bd-highlight">
                <div className="img_cont">
                    <img src={channel.image
                                ? channel.image
                                : "https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg"} className="rounded-circle user_img"/>
                    <span className="online_icon"></span>
                </div>
                <div className="user_info">
                    <span>{`Let's Chat: ${channel.name}`}</span>
                    {messages
                        ? <p>{`${messages.length} Message(s)`}</p>
                        : <></>}
                </div>
            </div>
        </div>
        <div className="card-body msg_card_body">
            {messages
                ? messages.map(message => { return methods.formatMessage(message, user)})
                : <></>}
            <div ref={messagesEndRef}/>
        </div >
        <div className="card-footer">
            <div className="input-group">
                <div className="input-group-append">
                    <span className="input-group-text attach_section h-100">
                    </span>
                </div>
                <textarea name="" className="form-control type_msg" onKeyDown={e => handleMessageSubmitWithEnterKey(e)} disabled={false} placeholder="Type your message..." onChange={e => handleMessageInput(e)} value={messageText}></textarea>
                <div className="input-group-append">
                    {/* <input type="submit" onClick={e => console.log("SUBMIT")}/> */}
                    <button className="input-group-text send_btn h-100" onClick={e => handleSendMessage(e)}>
                        <FontAwesomeIcon icon={faPaperPlane}
                        color={user.isPasswordValid
                            ? "#90EE90"
                            : "#ff4040"}
                        />
                    </button>
                </div>
            </div>
        </div>
    </div>
</>
}

export default Messages