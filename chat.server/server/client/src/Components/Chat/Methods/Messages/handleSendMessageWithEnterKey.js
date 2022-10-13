import handleSendMessage from "./handleSendMessage";

const handleSendMessageWithEnterKey = (e, user, messageText, jwt) => {
	if (!user.isPasswordValid) return
	if (messageText === "") return
	if (e.code === "Enter") handleSendMessage(e, messageText, jwt)
}

export default handleSendMessageWithEnterKey