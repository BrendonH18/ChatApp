import axios from 'axios'

const handleSendMessage = (e, messageText, jwt) => {
    e.preventDefault()
	// if (!user.isPasswordValid) return setMessageText("Message blocked - Please login to send messages")
    const message = {
      text: messageText,
      isBot: false
    }
	const authAxios = axios.create({
		baseURL: "https://localhost:44314/",
		headers: {
			Authorization: `Bearer ${jwt}`
		}
	})
	
    authAxios.post("api/chat/sendmessage", message)
	.then(res => console.log(res));
}

export default handleSendMessage